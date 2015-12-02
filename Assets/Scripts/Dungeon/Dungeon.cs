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
	private Hud hud;
	private Grid grid;
	private DungeonGenerator dungeonGenerator;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel = 0;


	void Awake () {
		navigator = Navigator.instance;
		sfx = AudioManager.instance;
		hud = Hud.instance;
		game = Game.instance;
		grid = Grid.instance;
		dungeonGenerator = DungeonGenerator.instance;
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
			hud.Log("You escaped the dungeon!");
			yield break;
		}

		// fade in
		yield return StartCoroutine(navigator.FadeIn(0.5f));
	}


	// =====================================================
	// Render Dungeon
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

		// Update game turn
		game.UpdateTurn();

		// Log welcome message
		hud.Log("Welcome to dungeon level " + currentDungeonLevel);
	}


	// =====================================================
	// Dungeon Architecture: Floors, Walls and Doors
	// =====================================================

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
						Tile tile = grid.CreateTile(typeof(Tile), x, y, 1, Game.assets.dungeon["floor-sandstone"]) as Tile;
	
						// set room info in floor tile
						if (dtile.room != null) {
							tile.roomId = dtile.room.id;
							//ile.SetInfo(tile.roomId.ToString(), dtile.room.color);
						}
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						Wall wall = grid.CreateEntity(typeof(Wall), x, y, 1, Game.assets.dungeon["floor-sandstone"]) as Wall;
						wall.SetColor(new Color(0.8f, 0.8f, 0.6f));
						//Generate3dWall(dtile, x, y);
					}
					
					// create doors
					if (dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						grid.CreateEntity(typeof(Door), x, y, 1, Game.assets.dungeon["door-closed"]);
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
	// Stairs generation
	// =====================================================

	private void GenerateStairs () {
		Tile tile = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(1);
		if (tile != null) {
			grid.stairUp = (Stair)grid.CreateEntity(typeof(Stair), tile.x, tile.y, 0.8f, Game.assets.dungeon["stairs-up"]) as Stair;
			grid.stairUp.SetDirection(-1);
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(1);
		if (tile != null) {
			grid.stairDown = (Stair)grid.CreateEntity(typeof(Stair), tile.x, tile.y, 0.8f, Game.assets.dungeon["stairs-down"]) as Stair;
			grid.stairDown.SetDirection(1);
		}
	}

	// =====================================================
	// Furniture generation
	// =====================================================

	private void GenerateFurniture () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxFurniture = Random.Range(0, 100) <= 85 ? Random.Range(1, (int)(room.tiles.Count * 0.3f)) : 0;

			// place furniture in room
			for (int i = 1; i <= maxFurniture; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				string[] arr = new string[] { 
					"barrel-closed", "barrel-open", "bed-h", "bed-v", "chair-h", "chair-v", 
					"chest-closed", "chest-open","fountain-fire", "fountain-water", 
					"grave-1", "grave-2", "grave-3", "lever-left", "lever-right", 
					"table-1", "vase" };

				grid.CreateEntity(typeof(Furniture), tile.x, tile.y, 0.8f, Game.assets.dungeon[arr[Random.Range(0, arr.Length)]]);
			}

			// tell the room that has been filled with furniture
			room.hasFurniture = true;
		}
	}

	// =====================================================
	// Monster generation
	// =====================================================

	private void GenerateMonsters () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {
			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxMonsters = Random.Range(0, 100) <= 50 ? Random.Range(1, (int)(room.tiles.Count * 0.2f)) : 0;

			//Color color = new Color (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			//PaintRoom(room, color);

			// Pick a random creature type
			List<System.Type> types = new List<System.Type>() { typeof(Goblin), typeof(Demon) };
			System.Type creatureType = types[0];

			//Sprite randomAsset = Game.assets.monster.ElementAt(Random.Range(0, Game.assets.monster.Count)).Value;

			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				grid.monsters.Add( grid.CreateCreature(creatureType, tile.x, tile.y, 0.8f)); // randomAsset, 
			}

			// tell the room that has been filled with monsters
			room.hasMonsters = true;
		}
	}


	// =====================================================
	// Player generation
	// =====================================================

	private void GeneratePlayer (int x, int y) {
		grid.player = grid.CreateCreature(typeof(Player), x, y, 0.8f, Game.assets.monster["adventurer"]) as Player;
		Camera.main.transform.position = new Vector3(grid.player.x, grid.player.y, -10);
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
