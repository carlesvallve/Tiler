using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (DungeonGenerator))]
[RequireComponent (typeof (Grid))]

public class Dungeon : MonoSingleton <Dungeon> {

	private Grid grid;
	private DungeonGenerator dungeonGenerator;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel = 0;

	//public delegate void DungeonGeneratedHandler(int dungeonLevel);
	//public event DungeonGeneratedHandler OnDungeonGenerated;


	void Awake () {
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
		yield return StartCoroutine(Navigator.instance.FadeOut(0.5f));
		GenerateDungeon(direction);
		yield return StartCoroutine(Navigator.instance.FadeIn(0.5f));
	}


	// =====================================================
	// Render dungeon in the game grid
	// =====================================================

	public void RenderDungeon (int direction = 1) {
		// init grid
		grid.InitializeGrid (dungeonGenerator.MAP_WIDTH, dungeonGenerator.MAP_HEIGHT);

		// generate grid elements for each tree quad
		GenerateGridOnTreeQuad(dungeonGenerator.quadTree);

		// Generate ladders
		GenerateLadders();

		//Generate furniture
		GenerateFurniture (20);

		//Generate monsters
		GenerateMonsters(10);

		// Generate player
		Tile ladder = direction == 1 ? grid.ladderUp : grid.ladderDown;
		GeneratePlayer(ladder.x, ladder.y);
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
						grid.CreateTile<Tile>(x, y, Game.assets.dungeon["floor-sandstone"], 1);
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
	// Wall generation
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
	// Creature generation
	// =====================================================

	private void GeneratePlayer (int x, int y) {
		grid.player = grid.CreateCreature<Creature>(x, y, Game.assets.monster["adventurer"], 0.8f);
		//grid.player.currentDungeonLevel = currentDungeonLevel;
	}


	private void GenerateMonsters (int max) {
		for (int i = 0; i < max; i ++) {
			Tile tile = GetRandomFreeTile(0);
			Sprite randomAsset = Game.assets.monster.ElementAt(Random.Range(0, Game.assets.monster.Count)).Value;
			grid.monsters.Add( grid.CreateCreature<Creature>(tile.x, tile.y, randomAsset, 0.8f) );
		}
	}

	// =====================================================
	// Ladder generation
	// =====================================================

	private void GenerateLadders () {
		Tile tile = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderUp = grid.CreateEntity<Entity>(tile.x, tile.y, Game.assets.dungeon["stairs-up"], 0.8f);
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderUp = grid.CreateEntity<Entity>(tile.x, tile.y, Game.assets.dungeon["stairs-down"], 0.8f);
		}
	}


	// =====================================================
	// Furniture generation
	// =====================================================

	private void GenerateFurniture (int max) {
		for (int i = 1; i <= max; i++) {
			Tile tile = GetRandomFreeTile(0);
			int c = 0;
			while (dungeonGenerator.tiles[tile.y, tile.x].id != DungeonTileType.ROOM && c < 100) {
				tile = GetRandomFreeTile(0);
			}

			string[] arr = new string[] { 
				"barrel-closed", "barrel-open", "bed-h", "bed-v", "chair-h", "chair-v", 
				"chest-closed", "chest-open","fountain-fire", "fountain-water", 
				"grave-1", "grave-2", "grave-3", "lever-left", "lever-right", 
				"table-1", "vase" };

			grid.CreateEntity<Entity>(tile.x, tile.y, Game.assets.dungeon[arr[Random.Range(0, arr.Length)]], 0.8f);
		}
	}


	// =====================================================
	// Feature generation Helpers
	// =====================================================

	// returns a tile which none of his neighbours at radius are occupied

	private Tile GetRandomFreeTile (int radius = 0) {
		

		Tile tile = null;
		Tile tile2 = null;
		int c = 0;

		while (true) {
			tile = grid.GetTile(
				Random.Range(0, grid.width), 
				Random.Range(0, grid.height)
			);

			bool ok = false;

			if (tile != null && !tile.IsOccupied()) {
				ok = true;

				// iterate on all surounding tiles
				for (int i = 1; i <= radius; i++) {
					tile2 = grid.GetTile(tile.x - i, tile.y);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }


					tile2 = grid.GetTile(tile.x - i, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y - i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x - i, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }

					tile2 = grid.GetTile(tile.x + i, tile.y + i);
					if (tile2 == null || (tile2 != null  && tile2.IsOccupied())) { ok = false; }
				}
			}

			// escape if tile is placeable
			if (ok) {
				break;
			}
			
			c++;
			if (c == 100) {
				Debug.LogError("Tile could not be placed. Escaping...");
				tile = null;
				break;
			}
		}

		return tile;
	}
}
