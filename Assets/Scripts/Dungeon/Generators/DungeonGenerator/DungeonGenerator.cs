using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DungeonGenerator : MonoSingleton <DungeonGenerator> {
	
	// Dungeon Parameters
	public int MAP_WIDTH = 64;
	public int MAP_HEIGHT = 64;
	
	// Room Parameters
	public int ROOM_MAX_SIZE = 24;
	public int ROOM_MIN_SIZE = 4;
	public int ROOM_WALL_BORDER = 1;
	public bool ROOM_UGLY_ENABLED = true; // used to eliminate ugly zones
	public float ROOM_MAX_RATIO = 5.0f;   // used to eliminate ugly zones
	
	// QuadTree Generation Parameters
	public int MAX_DEPTH = 10;
	public int CHANCE_STOP = 5;
	public int SLICE_TRIES = 10;

	// Corridor Generation Parameters
	public int CORRIDOR_WIDTH = 2;
	
	// Tilemap
	public DungeonTile[,] tiles;
	
	// The Random Seed
	public int seed = -1;
	
	// QuadTree for dungeon distribution
	public QuadTree quadTree;
	
	// List of rooms
	public List<DungeonRoom> rooms;
	
	// Auxiliar vars
	public bool verbose = false;
	

	// *************************************************************
	// Dungeon Initialization
	// *************************************************************

	public void Reset () {
		// Initialize the tilemap
		tiles = new DungeonTile[MAP_HEIGHT, MAP_WIDTH];

		for (int y = 0; y < MAP_HEIGHT; y++) {
			for (int x = 0; x < MAP_WIDTH; x++) {
				tiles[y, x] = new DungeonTile(DungeonTileType.EMPTY, x, y);
			}
		}
		
		// Initialize the QuadTree
		quadTree = new QuadTree(new AABB(new XY(MAP_WIDTH / 2f, MAP_HEIGHT / 2f), new XY(MAP_WIDTH / 2f, MAP_HEIGHT / 2f)));

		//  Initialize the list of rooms
		rooms = new List<DungeonRoom>();
	}

	
	// Generate a new dungeon with the given seed
	public void Generate(int seed) {
		// set and store random seed
		this.seed = seed;

		// Clean
		Reset();
			
		// Generate QuadTree
		if (verbose) { Debug.Log ("Generating QuadTree"); }
		GenerateQuadTree (ref quadTree);
		
		// Generate Rooms
		if (verbose) { Debug.Log ("Generating Rooms"); }
		GenerateRooms (ref rooms, quadTree);
		
		// Generate Corridors
		if (verbose) { Debug.Log ("Generating Corridors"); }
		GenerateCorridors ();
		
		if (verbose) { Debug.Log ("Generating Walls"); }
		GenerateWalls();

		if (verbose) { Debug.Log ("Generating Doors"); }
		GenerateDoors();
	}


	// *************************************************************
	// Generate Dungeon Features
	// *************************************************************
	
	// Generate the quadtree system
	void GenerateQuadTree(ref QuadTree _quadTree) {
		_quadTree.GenerateQuadTree(seed);
	}

	
	// Generate the list of rooms and dig them
	public void GenerateRooms(ref List<DungeonRoom> _rooms, QuadTree _quadTree) {
		// Childless node
		if (_quadTree.northWest == null && _quadTree.northEast == null && _quadTree.southWest == null && _quadTree.southEast == null) {
			_rooms.Add(GenerateRoom(_rooms.Count, _quadTree));
			return;
		}
		
		// Recursive call
		if (_quadTree.northWest != null) GenerateRooms (ref _rooms,_quadTree.northWest);
		if (_quadTree.northEast != null) GenerateRooms (ref _rooms,_quadTree.northEast);
		if (_quadTree.southWest != null) GenerateRooms (ref _rooms,_quadTree.southWest);
		if (_quadTree.southEast != null) GenerateRooms (ref _rooms,_quadTree.southEast);
	}

	
	// Generate a single room
	public DungeonRoom GenerateRoom(int id, QuadTree _quadTree) {
		// Center of the room
		XY roomCenter = new XY();
		roomCenter.x = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Left() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Right() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);
		roomCenter.y = Random.Range(ROOM_WALL_BORDER + _quadTree.boundary.Bottom() + ROOM_MIN_SIZE/2.0f, _quadTree.boundary.Top() - ROOM_MIN_SIZE/2.0f - ROOM_WALL_BORDER);		
		
		// Half size of the room
		XY roomHalf = new XY();
		
		float halfX = (_quadTree.boundary.Right() - roomCenter.x - ROOM_WALL_BORDER);
		float halfX2 =(roomCenter.x - _quadTree.boundary.Left() - ROOM_WALL_BORDER);
		if (halfX2 < halfX) halfX = halfX2;
		if (halfX > ROOM_MAX_SIZE/2.0f) halfX = ROOM_MAX_SIZE/2.0f;
		
		float halfY = (_quadTree.boundary.Top() - roomCenter.y - ROOM_WALL_BORDER);
		float halfY2 =(roomCenter.y - _quadTree.boundary.Bottom() - ROOM_WALL_BORDER);
		if (halfY2 < halfY) halfY = halfY2;
		if (halfY > ROOM_MAX_SIZE/2.0f) halfY = ROOM_MAX_SIZE/2.0f;
		
		roomHalf.x = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfX);
		roomHalf.y = Random.Range((float)ROOM_MIN_SIZE/2.0f,halfY);

		// Eliminate ugly zones
		if (ROOM_UGLY_ENABLED == false) {
			float aspect_ratio = roomHalf.x / roomHalf.y;
			if (aspect_ratio > ROOM_MAX_RATIO || aspect_ratio < 1.0f/ROOM_MAX_RATIO) return GenerateRoom(id, _quadTree); 
		}
		
		// Create AABB
		AABB randomAABB = new AABB(roomCenter, roomHalf);

		// create room
		DungeonRoom room = new DungeonRoom(id, randomAABB, _quadTree);
		
		// Dig the room in our tilemap
		DigRoom (room, randomAABB.BottomTile(), randomAABB.LeftTile(), randomAABB.TopTile()-1, randomAABB.RightTile()-1);
		
		// Return the room
		return room;
	}

	
	void GenerateCorridors() {
		quadTree.GenerateCorridors();
	}


	// Generate walls when there's something near
	public void GenerateWalls() {
		// Place walls
		for (int y = 0; y < MAP_HEIGHT; y++) {
			for (int x = 0; x < MAP_WIDTH; x++) {
				bool room_near = false;
				if (IsPassable(x,y)) continue;

				if (x > 0) if (IsPassable(x - 1, y)) room_near = true;
				if (x < MAP_WIDTH - 1) if (IsPassable(x + 1, y)) room_near = true;
				if (y > 0) if (IsPassable(x, y - 1)) room_near = true;
				if (y < MAP_HEIGHT - 1) if (IsPassable(x, y + 1)) room_near = true;
				if (room_near) SetWall(x, y);
			}
		}

		// place wall corners
		for (int y = 0; y < MAP_HEIGHT; y++) {
			for (int x = 0; x < MAP_WIDTH; x++) {
				if (IsWallCorner(x, y)) SetWallCorner(x, y);
			}
		}

		// fix any walls missing on the map borders
		GenerateWallsOnMapBorders();
	}


	public void GenerateWallsOnMapBorders () {
		// Place walls in case rooms ended up open as a result of ROOM_WALL_BORDER = 0
		for (int y = 0; y < MAP_HEIGHT; y++) {
			for (int x = 0; x < MAP_WIDTH; x++) {
				if (x == 0 || y == 0 || x == MAP_WIDTH - 1 || y == MAP_HEIGHT - 1) {
					DungeonTile tile = tiles[y, x];
					if (!tile.isEmpty()) {
						SetWall(x, y);
						if (x == 0 && tiles[y, x + 1].isWall()) { SetWallCorner(x, y); }
						if (y == 0 && tiles[y + 1, x].isWall()) { SetWallCorner(x, y); }
						if (x == MAP_WIDTH - 1 && tiles[y, x - 1].isWall()) { SetWallCorner(x, y); }
						if (y == MAP_HEIGHT - 1 && tiles[y - 1, x].isWall()) { SetWallCorner(x, y); }
						if (x == 0 && y == 0) { SetWallCorner(x, y); }
						if (x == MAP_WIDTH - 1 && y == MAP_HEIGHT - 1) { SetWallCorner(x, y); }
					}
				}
			}
		}
	}


	public void GenerateDoors() {
		List<DungeonTile> doorsH = new List<DungeonTile>();
		List<DungeonTile> doorsV = new List<DungeonTile>();

		// Define at which tiles doors should be placed
		for (int y = 1; y < MAP_HEIGHT - 1; y++) {
			for (int x = 1; x < MAP_WIDTH - 1; x++) {
				DungeonTile tile = tiles[y, x];
				if (tile.id != DungeonTileType.CORRIDOR) continue;

				// Vertical doors
				if (isDoorWall(tiles[y - 1, x].id) || isDoorWall(tiles[y + 1, x].id)) {
					if (tiles[y, x + 1].id == DungeonTileType.ROOM && tiles[y, x - 1].id == DungeonTileType.CORRIDOR) {
						doorsV.Add(tiles[y, x]);
					}

					if (tiles[y, x - 1].id == DungeonTileType.ROOM && tiles[y, x + 1].id == DungeonTileType.CORRIDOR) {
						doorsV.Add(tiles[y, x]);
					}
				}

				// Horizontal doors
				if (isDoorWall(tiles[y, x - 1].id) || isDoorWall(tiles[y, x + 1].id)) {
					if (tiles[y + 1, x].id == DungeonTileType.ROOM && tiles[y - 1, x].id == DungeonTileType.CORRIDOR) {
						doorsH.Add(tiles[y, x]);
					}

					if (tiles[y - 1, x].id == DungeonTileType.ROOM && tiles[y + 1, x].id == DungeonTileType.CORRIDOR) {
						doorsH.Add(tiles[y, x]);
					}
				}
			}
		}

		// set those tiles as doors
		foreach (DungeonTile tile in doorsH) {
			tile.id = DungeonTileType.DOORH;
		}

		foreach (DungeonTile tile in doorsV) {
			tile.id = DungeonTileType.DOORV;
		}


		// remove any bad or undesired  doors
		for (int y = 1; y < MAP_HEIGHT - 1; y++) {
			for (int x = 1; x < MAP_WIDTH - 1; x++) {
				DungeonTile tile = tiles[y, x];
				
				// remove undesired doors
				if (tile.id == DungeonTileType.DOORV) {
					if (isDoorFloor(tiles[y - 1, x].id) || isDoorFloor(tiles[y + 1, x].id)) {
						tile.id = DungeonTileType.CORRIDOR;
					}

					if (isDoorDoor(tiles[y, x - 1].id) || isDoorDoor(tiles[y, x + 1].id)) {
						tile.id = DungeonTileType.CORRIDOR;
					}
				}

				if (tile.id == DungeonTileType.DOORH) {
					if (isDoorFloor(tiles[y, x - 1].id) || isDoorFloor(tiles[y, x + 1].id)) {
						tile.id = DungeonTileType.CORRIDOR;
					}
					if (isDoorDoor(tiles[y - 1, x].id) || isDoorDoor(tiles[y + 1, x].id)) {
						tile.id = DungeonTileType.CORRIDOR;
					}
				}
			}
		}
	}


	private bool isDoorWall(DungeonTileType id) {
		return id == DungeonTileType.WALL || id == DungeonTileType.WALLCORNER;
	}


	private bool isDoorFloor(DungeonTileType id) {
		return 
		id == DungeonTileType.CORRIDOR || id == DungeonTileType.ROOM;
	}


	private bool isDoorDoor (DungeonTileType id) {
		return id == DungeonTileType.DOORH || id == DungeonTileType.DOORV;
	}


	// *************************************************************
	// Helper Methods
	// *************************************************************

	public bool IsEmpty(int x, int y) { 
		return tiles[y, x].id == DungeonTileType.EMPTY; 
	}
	

	public bool IsPassable(int x, int y) { 
		return 
			tiles[y, x].id == DungeonTileType.ROOM || 
			tiles[y, x].id == DungeonTileType.CORRIDOR;
	}

	
	public bool IsPassable(XY xy) { 
		return IsPassable((int) xy.x, (int) xy.y);
	}


	public bool IsWallCorner(int x, int y) { 
		if (tiles[y, x].id != DungeonTileType.EMPTY) { return false; }

		if (y > 0 && x > 0 && 
			tiles[y - 1, x].id == DungeonTileType.WALL && tiles[y, x - 1].id == DungeonTileType.WALL && tiles[y - 1, x - 1].id != DungeonTileType.WALL) { 
			return true; 
		}
		
		if (y > 0 && x < MAP_WIDTH - 1 && 
			tiles[y - 1, x].id == DungeonTileType.WALL && tiles[y, x + 1].id == DungeonTileType.WALL && tiles[y - 1, x + 1].id != DungeonTileType.WALL) { 
			return true; 
		}
		
		if (y < MAP_HEIGHT - 1 && x > 0 && 
			tiles[y + 1, x].id == DungeonTileType.WALL && tiles[y, x - 1].id == DungeonTileType.WALL && tiles[y + 1, x - 1].id != DungeonTileType.WALL) { 
			return true; 
		}
		
		if (y < MAP_HEIGHT - 1 && x < MAP_WIDTH - 1 && 
			tiles[y + 1, x].id == DungeonTileType.WALL && tiles[y, x + 1].id == DungeonTileType.WALL && tiles[y + 1, x + 1].id != DungeonTileType.WALL) { 
			return true; 
		}
		
		return false;
	}

	
	public void SetWall(int x, int y) {
		tiles[y, x].id = DungeonTileType.WALL;
	}


	public void SetWallCorner(int x, int y) {
		tiles[y, x].id = DungeonTileType.WALLCORNER;
	}


	public void SetDoor(int x, int y, string direction) {
		tiles[y, x].id = direction == "h" ? DungeonTileType.DOORH : DungeonTileType.DOORV;
	}


	// Dig a room, placing floor tiles
	public void DigRoom(DungeonRoom room, int row_bottom, int col_left, int row_top, int col_right) {
		// Out of range
		if ( row_top < row_bottom ) {
		    int tmp = row_top;
		    row_top = row_bottom;
		    row_bottom = tmp;
		}
		
		// Out of range
		if ( col_right < col_left ) {
		    int tmp = col_right;
		    col_right = col_left;
		    col_left = tmp;
		}
		
		if (row_top > MAP_HEIGHT-1) return;
		if (row_bottom < 0) return;
		if (col_right > MAP_WIDTH-1) return;
		if (col_left < 0) return;
		
		// Dig floor
	    for (int row = row_bottom; row <= row_top; row++) 
	        for (int col = col_left; col <= col_right; col++) 
	            DigRoom (room, row,col);
	}
	

	public void DigRoom(DungeonRoom room, int row, int col) {
		DungeonTile tile = tiles[row,col];
		tile.id = DungeonTileType.ROOM;
		tile.room = room;
		tile.color = room.color;

		room.tiles.Add(tile);
	}

	
	public void DigCorridor(int row, int col) {
		if (tiles[row,col].id != DungeonTileType.ROOM) {
			tiles[row,col].id = DungeonTileType.CORRIDOR;
		}
	}

	
	public void DigCorridor(XY p1, XY p2) {
		int row1 = Mathf.RoundToInt(p1.y);
		int row2 = Mathf.RoundToInt(p2.y);
		int col1 = Mathf.RoundToInt(p1.x);
		int col2 = Mathf.RoundToInt(p2.x);
		
		DigCorridor(row1,col1,row2,col2);
	}

	
	public void DigCorridor(int row1, int col1, int row2, int col2) {		
		if (row1 <= row2) {
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row1; row <= row2; row++)
					DigCorridor(row,col);
		} else {
			for (int col = col1; col < col1 + CORRIDOR_WIDTH; col++)
				for (int row = row2; row <= row1; row++)
					DigCorridor(row,col);
		}
		
		if (col1 <= col2) {
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col1; col <= col2; col++)
					DigCorridor(row,col);
		} else {
			for (int row = row2; row < row2 + CORRIDOR_WIDTH; row++)
				for (int col = col2; col <= col1; col++)
					DigCorridor(row2,col);
		}
	}


	// *************************************************************
	// Debug Logs
	// *************************************************************

	public void logGrid () {
		print("Grid " + MAP_WIDTH + ", " + MAP_HEIGHT);

		string str = "";
		for (int y = 0; y < MAP_HEIGHT; y++) {
			
			for (int x = 0; x < MAP_WIDTH; x++) {
				DungeonTile tile = tiles[x, y];
				str += tile.getWalkable() ? "1" : "0"; //id;
			}
			str += "\n";
		}

		print (str);
	}


	public void logRooms () {
		print ("Rooms: " + rooms.Count);

		for (int i = 0; i < rooms.Count; i++) {
			DungeonRoom room = rooms[i];
			print ("    Room" + room.id +  " (" + room.tiles.Count + " tiles)");
		}
	}


	// *************************************************************
	// Paint textures for debug purposes
	// *************************************************************

	/*private Texture2D DungeonToTexture() {
		Texture2D texOutput = new Texture2D((int) (MAP_WIDTH), (int) (MAP_HEIGHT),TextureFormat.ARGB32, false);
		PaintDungeonTexture(ref texOutput);
		texOutput.filterMode = FilterMode.Point;
		texOutput.wrapMode = TextureWrapMode.Clamp;
		texOutput.Apply();
		return texOutput;
	}

	private void PaintDungeonTexture(ref Texture2D t) {
		for (int i = 0; i < MAP_WIDTH; i++) for (int j = 0; j < MAP_HEIGHT; j++) {
			switch (tiles[j,i].id) {
			case DungeonTileType.EMPTY:
				t.SetPixel(i,j,Color.black);
				break;
			case DungeonTileType.ROOM:
				t.SetPixel(i,j,Color.white);
				break;
			case DungeonTileType.CORRIDOR:
				t.SetPixel(i,j,Color.grey);
				break;
			case DungeonTileType.WALL:
				t.SetPixel(i,j,Color.blue);
				break;
			case DungeonTileType.WALLCORNER:
				t.SetPixel(i,j,Color.blue);
				break;
			}
		}
	}
	
	// Export a texture to a file
	private public void TextureToFile(Texture2D t, string filename) {
		Directory.CreateDirectory(Path.GetDirectoryName(path));

		byte[] bytes = t.EncodeToPNG();
		FileStream myFile = new FileStream(Application.dataPath + "/Resources/_debug/" + filename + ".png",FileMode.OpenOrCreate,System.IO.FileAccess.ReadWrite);
		myFile.Write(bytes,0,bytes.Length);
		myFile.Close();
	}*/
	
}
