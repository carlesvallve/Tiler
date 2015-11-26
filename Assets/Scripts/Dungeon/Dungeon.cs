using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Dungeon : MonoBehaviour {

	private Grid grid;
	private DungeonGenerator dungeonGenerator;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel = -1;

	public delegate void DungeonGeneratedHandler(int dungeonLevel);
	public event DungeonGeneratedHandler OnDungeonGenerated;


	void Awake () {
		grid = GetComponent<Grid>();
		dungeonGenerator = GetComponent<DungeonGenerator>();
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
	// Generate Dungeon
	// =====================================================

	public void GenerateDungeon (int direction) {
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
		//dungeonRenderer.Init(dungeonGenerator, grid);
		RenderDungeonToGrid();

		// Generate ladders
		GenerateLadders();

		// Generate player
		Ladder ladder = direction == 1 ? grid.ladderUp : grid.ladderDown;
		GeneratePlayer(ladder.x, ladder.y - 1);

		// emit event
		if (OnDungeonGenerated != null) {
			OnDungeonGenerated.Invoke(currentDungeonLevel);
		}
	}


	// =====================================================
	// Render dungeon in the game grid
	// =====================================================

	private void RenderDungeonToGrid () {
		// init grid
		grid.InitializeGrid (dungeonGenerator.MAP_WIDTH, dungeonGenerator.MAP_HEIGHT);

		// generate grid elements for each tree quad
		GenerateGridOnTreeQuad(dungeonGenerator.quadTree);
		
		// batch all grid elements
		grid.BatchGrid();
	}


	private void GenerateGridOnTreeQuad (QuadTree _quadtree) {
		if (_quadtree.HasChildren() == false) {

			for (int y = _quadtree.boundary.BottomTile(); y <= _quadtree.boundary.TopTile() - 1; y++) {
				for (int x = _quadtree.boundary.LeftTile(); x <= _quadtree.boundary.RightTile() - 1; x++) {
					// get dungeon tile on the quadtree zone
					DungeonTile dtile = dungeonGenerator.tiles[y, x];

					// set render color 
					Color color = _quadtree.color;
	
					// create floors
					if (dtile.id == DungeonTileType.ROOM || dtile.id == DungeonTileType.CORRIDOR || 
						dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						grid.CreateTile(x, y, TileTypes.Floor, color);
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						float d = 0.9f;
						Color wallColor = new Color(color.r * d, color.g * d, color.b * d, 1f);
						grid.CreateTile(x, y, TileTypes.Floor, wallColor);
						grid.CreateObstacle(x, y, ObstacleTypes.Wall, wallColor);
					}

					// create doors
					if (dtile.id == DungeonTileType.DOORH) {
						grid.CreateDoor(x, y, DoorTypes.Wood, DoorStates.Closed, DoorDirections.Horizontal);
					}

					if (dtile.id == DungeonTileType.DOORV) {
						grid.CreateDoor(x, y, DoorTypes.Wood, DoorStates.Closed, DoorDirections.Vertical);
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
	// Generate Dungeon features inside the game grid
	// =====================================================

	private void GeneratePlayer (int x, int y) {
		grid.player = grid.CreatePlayer(x, y, PlayerTypes.Player);
		grid.player.currentDungeonLevel = currentDungeonLevel;
	}


	private void GenerateLadders () {
		Tile tile = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderUp = grid.CreateLadder(tile.x, tile.y, LadderTypes.Wood, LadderDirections.Up);
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetRandomFreeTile(1);
		if (tile != null) {
			grid.ladderDown = grid.CreateLadder(tile.x, tile.y, LadderTypes.Wood, LadderDirections.Down);
		}
	}


	// =====================================================
	// Feature generation Helpers
	// =====================================================

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
