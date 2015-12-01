using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (DungeonGenerator))]
[RequireComponent (typeof (Grid))]

public class Dungeon : MonoSingleton <Dungeon> {
	private Navigator navigator;
	private AudioManager sfx;
	private Game game;
	private Grid grid;
	private DungeonGenerator dungeonGenerator;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel = 0;

	//public delegate void DungeonGeneratedHandler(int dungeonLevel);
	//public event DungeonGeneratedHandler OnDungeonGenerated;


	void Awake () {
		navigator = Navigator.instance;
		sfx = AudioManager.instance;
		game = GetComponent<Game>();
		grid = GetComponent<Grid>();
		dungeonGenerator = GetComponent<DungeonGenerator>();
	}


	// =====================================================
	// Generate Dungeon
	// =====================================================

	public void GenerateDungeon (int direction = 0) {
		// Update current dungeon level
		currentDungeonLevel += direction;

		// Set random seed
		int seed;
		if (currentDungeonLevel > dungeonSeeds.Count - 1) {
			// Set a random seed if we are entering a new dungeon level
			seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
			dungeonSeeds.Add(seed);
		} else {
			// Recover a previously stored seed on current dungeon level
			seed = dungeonSeeds[currentDungeonLevel];
		}

		// Apply random seed
		dungeonGenerator.seed = seed;
		Random.seed = seed;
		
		// Generate dungeon data
		dungeonGenerator.GenerateDungeon(dungeonGenerator.seed);

		// Render dungeon on grid
		RenderDungeon(direction);

		//sfx.Play("Audio/Sfx/Step/step", 1f, Random.Range(0.8f, 1.2f));
		sfx.Play("Audio/Sfx/Musical/gong", 0.6f, Random.Range(0.8f, 1.2f));

		// emit event
		/*if (OnDungeonGenerated != null) {
			OnDungeonGenerated.Invoke(currentDungeonLevel);
		}*/
	}


	// =====================================================
	// Navigate Dungeon Levels
	// =====================================================

	public void ExitLevel (int direction) {
		StartCoroutine(ExitLevelCoroutine(direction));
	}

	
	private  IEnumerator ExitLevelCoroutine (int direction) {
		game.CrossFadeRandomBgm();

		// fade out
		yield return StartCoroutine(navigator.FadeOut(0.5f));
		
		// generate next dungeon level
		if (currentDungeonLevel + direction >= 0) {
			GenerateDungeon(direction);
		} else {
			grid.ResetGrid();
			print ("You escaped the dungeon!");
			yield break;
		}

		// fade in
		yield return StartCoroutine(navigator.FadeIn(0.5f));
	}


	// =====================================================
	// Render dungeon data on grid
	// =====================================================

	public void RenderDungeon (int direction) {
		// init grid
		grid.InitializeGrid (dungeonGenerator.MAP_WIDTH, dungeonGenerator.MAP_HEIGHT);

		// generate grid elements for each tree quad
		GenerateGridOnTreeQuad(dungeonGenerator.quadTree);

		// Generate ladders
		GenerateStairs();

		//Generate furniture
		GenerateFurniture ();

		//Generate monsters
		GenerateMonsters();

		// Generate player
		Stair stair = direction == -1 ? grid.stairDown : grid.stairUp;
		GeneratePlayer(stair.x, stair.y);

	}


	private void GenerateGridOnTreeQuad (QuadTree _quadtree) {
		if (_quadtree.HasChildren() == false) {

			for (int y = _quadtree.boundary.BottomTile(); y <= _quadtree.boundary.TopTile() - 1; y++) {
				for (int x = _quadtree.boundary.LeftTile(); x <= _quadtree.boundary.RightTile() - 1; x++) {
					// get dungeon tile on the quadtree zone
					DungeonTile dtile = dungeonGenerator.tiles[y, x];

					// set render color 
					//Color color = _quadtree.color;
	
					// create floors
					if (dtile.id == DungeonTileType.ROOM || dtile.id == DungeonTileType.CORRIDOR || 
						dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						Tile tile = grid.CreateTile<Tile>(x, y, Game.assets.dungeon["floor-sandstone"], 1);
	
						// set room info in floor tile
						if (dtile.room != null) {
							tile.roomId = dtile.room.id;
							//ile.SetInfo(tile.roomId.ToString(), dtile.room.color);
						}
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						Wall wall = grid.CreateEntity<Wall>(x, y, Game.assets.dungeon["floor-sandstone"], 1);
						wall.SetColor(new Color(0.8f, 0.8f, 0.6f));
						//Generate3dWall(dtile, x, y);
					}
					
					// create doors
					if (dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						grid.CreateEntity<Door>(x, y, Game.assets.dungeon["door-closed"], 1);
					}
				}
			}
		} else {
			// Keep iterating on the quadtree
			GenerateGridOnTreeQuad(_quadtree.northWest);
			GenerateGridOnTreeQuad(_quadtree.northEast);
			GenerateGridOnTreeQuad(_quadtree.southWest);
			GenerateGridOnTreeQuad(_quadtree.southEast);
		}
	}


	// =====================================================
	// Wall 3d generation
	// =====================================================

	/*private void Generate3dWall (DungeonTile dtile, int x, int y) {
		// create 3d walls
		if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {	
			//float d = 0.9f;
			//Color wallColor = new Color(color.r * d, color.g * d, color.b * d, 1f);

			Sprite wallAsset;
			if (IsVerticalWall(dtile, x, y)) {
				wallAsset = Game.assets.dungeon["wall-v"];
			} else {
				wallAsset = Game.assets.dungeon["wall-h"];
			}
			//grid.CreateEntity(x, y, wallAsset);
			grid.CreateEntity<Wall>(x, y, wallAsset, 1); // 
		}
	}


	private bool IsFloor (DungeonTile dtile) {
		if (dtile.id == DungeonTileType.ROOM || dtile.id == DungeonTileType.CORRIDOR || 
			dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
			return true;
		}

		return false;
	}


	private bool IsVerticalWall (DungeonTile dtile, int x, int y) {
		if (y <= 0) { 
			return false;
		}

		DungeonTile bottomTile = dungeonGenerator.tiles[y - 1, x]; 
		if (bottomTile.id == DungeonTileType.EMPTY || IsFloor(bottomTile)) {
			return false;
		}

		return true;
	}*/


	// =====================================================
	// Stairs generation
	// =====================================================

	private void GenerateStairs () {
		Tile tile = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(1);
		if (tile != null) {
			grid.stairUp = grid.CreateEntity<Stair>(tile.x, tile.y, Game.assets.dungeon["stairs-up"], 0.8f);
			grid.stairUp.SetDirection(-1);
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(1);
		if (tile != null) {
			grid.stairDown = grid.CreateEntity<Stair>(tile.x, tile.y, Game.assets.dungeon["stairs-down"], 0.8f);
			grid.stairDown.SetDirection(1);
		}
	}

	// =====================================================
	// Furniture generation
	// =====================================================

	private void GenerateFurniture () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxFurniture = Random.Range(0, 100) <= 85 ? Random.Range(1, (int)(room.tiles.Count / 3)) : 0;

			// get random room suitable for placing furniture
			/*DungeonRoom room = GetRandomRoom();

			int c = 0;
			while (room.hasFurniture) {
				room = GetRandomRoom();
				c++;
				if (c == 1000) { 
					print ("No room is suitable to place furniture in it. Aborting.");
					return; 
				}
			}*/

			// place furniture in room
			for (int i = 1; i <= maxFurniture; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				string[] arr = new string[] { 
					"barrel-closed", "barrel-open", "bed-h", "bed-v", "chair-h", "chair-v", 
					"chest-closed", "chest-open","fountain-fire", "fountain-water", 
					"grave-1", "grave-2", "grave-3", "lever-left", "lever-right", 
					"table-1", "vase" };

				grid.CreateEntity<Entity>(tile.x, tile.y, Game.assets.dungeon[arr[Random.Range(0, arr.Length)]], 0.8f);
			}

			// tell the room that has been filled with furniture
			room.hasFurniture = true;
		}
	}

	// =====================================================
	// Creature generation
	// =====================================================

	private void GeneratePlayer (int x, int y) {
		grid.player = grid.CreateCreature<Creature>(x, y, Game.assets.monster["adventurer"], 0.8f);
		Camera.main.transform.position = new Vector3(grid.player.x, grid.player.y, -10);
	}


	private void GenerateMonsters () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {
			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxMonsters = Random.Range(0, 100) <= 50 ? Random.Range(1, (int)(room.tiles.Count / 3)) : 0;

			// get random room suitable for placing mosters
			/*DungeonRoom room = GetRandomRoom();

			int c = 0;
			while (room.hasMonsters ) {
				room = GetRandomRoom();
				c++;
				if (c == 1000) { 
					print ("No room is suitable to place monsters in it. Aborting.");
					return; 
				}
			}*/

			//Color color = new Color (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			//PaintRoom(room, color);

			// TODO: find a way to pick a random class (using reflection?)
			//System.Type randomCreatureClass 
			//List<System.Type> types = new List<System.Type>() { typeof(Goblin), typeof(Demon) };
			//System.Type creatureType = types[0];
			//print (">>> " + creatureType);

			Sprite randomAsset = Game.assets.monster.ElementAt(Random.Range(0, Game.assets.monster.Count)).Value;

			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				grid.monsters.Add( grid.CreateCreature<Creature>(tile.x, tile.y, randomAsset, 0.8f) );
			}

			// tell the room that has been filled with monsters
			room.hasFurniture = true;
		}
	}


	


	// =====================================================
	// Feature generation Helpers
	// =====================================================

	private DungeonRoom GetRandomRoom (bool debug = false) {
		DungeonRoom room = dungeonGenerator.rooms[Random.Range(0, dungeonGenerator.rooms.Count)];
		if (debug) { PaintRoom(room, Color.black); }
		return room;
	}


	private void PaintRoom (DungeonRoom room, Color color) {
		foreach (DungeonTile dtile in room.tiles) {
			Tile tile = grid.GetTile(dtile.x, dtile.y);
			if (tile != null) {
				tile.SetColor(color);
			}
		}
	}


	private Tile GetFreeTileOnRoom (DungeonRoom room, int radius = 0) {
		// get a random free tile inside the given room

		int c = 0;
		while (true) {
			DungeonTile dtile = room.tiles[Random.Range(0, room.tiles.Count)];
			Tile tile = grid.GetTile(dtile.x, dtile.y);
			if (tile != null && TileIsFree(tile, radius)) {
				return tile;
			}

			c++; if (c == 1000) { break; }
		}

		Debug.LogError("Tile could not be placed. Escaping...");
		return null;
	}


	private Tile GetFreeTileOnGrid (int radius = 0) {
		// get a random free tile inside the grid

		int c = 0;
		while (true) {
			Tile tile = grid.GetTile(Random.Range(0, grid.width), Random.Range(0, grid.height));

			if (tile != null && TileIsFree(tile, radius)) {
				return tile;
			}

			c++; if (c == 1000) { break; }
		}

		Debug.LogError("Tile could not be placed. Escaping...");
		return null;
	}


	private bool TileIsFree (Tile tile, int radius) {
		// make sure that given tile, and all tiles around in given radius are not occupied

		if (tile != null && !tile.IsOccupied()) {
			Tile tile2 = null;

			// iterate on all surounding tiles
			for (int i = 1; i <= radius; i++) {
				tile2 = grid.GetTile(tile.x - i, tile.y);

				// horizontal
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x + i, tile.y);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x, tile.y - i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x, tile.y + i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				// diagonal
				tile2 = grid.GetTile(tile.x - i, tile.y - i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x + i, tile.y - i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x - i, tile.y + i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }

				tile2 = grid.GetTile(tile.x + i, tile.y + i);
				if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { return false; }
			}

			return true;
		}

		return false;
	}
}
