using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonFeatureGenerator {

	protected DungeonGenerator dungeonGenerator;
	protected Dungeon dungeon;
	protected Grid grid;


	public DungeonFeatureGenerator () {
		dungeonGenerator = DungeonGenerator.instance;
		dungeon  = Dungeon.instance;
		grid = Grid.instance;
	}


	public virtual void Generate () {
	}

	
	public virtual void Generate (int chancePerRoom, float ratioPerFreeTiles) {
	}


	public virtual void Generate (Tile tile, int maxItems, int minRarity = 100) {
	}


	public virtual void GenerateAtPos (int x, int y) {
	}


	// =====================================================
	// Feature generation Helpers
	// =====================================================

	protected DungeonRoom GetRandomRoom (bool debug = false) {
		DungeonRoom room = dungeonGenerator.rooms[Random.Range(0, dungeonGenerator.rooms.Count)];
		if (debug) { PaintRoom(room, Color.black); }
		return room;
	}


	protected void PaintRoom (DungeonRoom room, Color color) {
		foreach (DungeonTile dtile in room.tiles) {
			Tile tile = grid.GetTile(dtile.x, dtile.y);
			if (tile != null) {
				tile.SetColor(color);
			}
		}
	}


	protected Tile GetFreeTileOnGrid (int radius = 0) {
		List<Tile> freeTiles = GetFreeTilesOnGrid(radius);

		if (freeTiles.Count == 0) {
			//Debug.LogError("No free tile available in grid. Escaping...");
			return null;
		}

		return freeTiles[Random.Range(0, freeTiles.Count)];
	}


	protected Tile GetFreeTileOnRoom (DungeonRoom room, int radius = 0) {
		List<Tile> freeTiles =GetFreeTilesOnRoom(room, radius);

		if (freeTiles.Count == 0) {
			//Debug.LogError("No free tile available in grid. Escaping...");
			return null;
		}

		return freeTiles[Random.Range(0, freeTiles.Count)];
	}


	public List<Tile> GetFreeTilesOnGrid (int radius = 0) {
		List<Tile> freeTiles = new List<Tile>();
		
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {
				Tile tile = grid.GetTile(x, y);
				if (tile != null && TileIsFree(tile, radius)) {
					freeTiles.Add(tile);
				}
			}
		}

		return freeTiles;
	}


	public List<Tile> GetFreeTilesOnRoom (DungeonRoom room, int radius) {
		List<Tile> freeTiles = new List<Tile>();
		
		foreach (DungeonTile dtile in room.tiles) {
			Tile tile = grid.GetTile(dtile.x, dtile.y);
			if (tile != null && TileIsFree(tile, radius)) {
				freeTiles.Add(tile);
			}
		}

		return freeTiles;
	}


	protected bool TileIsFree (Tile tile, int radius) {
		// make sure that given tile, and all tiles around in given radius are not occupied

		// if there is not tile, tile is not free
		if (tile == null) { return false; }

		// if tile is occupied, tile is not free
		if (tile.IsOccupied()) { return false; }

		// if any tile inside given radius is occupied, tile is not free
		List<Tile> tiles = grid.GetNeighboursInsideRadius(tile.x, tile.y, radius);
		foreach (Tile neighbour in tiles) {
			if (neighbour == null) { return false; }
			if (neighbour.IsOccupied()) { return false; }
		}

		// there is a door adjacent to the tile in 4 directions, tile is not free
		if (IsAdjacentToDoor(tile.x, tile.y)) { return false; }

		// otherwise, tile is free
		return true;
	}


	public bool IsAdjacentToDoor (int x, int y) {
		List<Entity> neighbours = new List<Entity>() {
			grid.GetEntity(x + 0, y - 1),
			grid.GetEntity(x + 1, y + 0),
			grid.GetEntity(x + 0, y + 1),
			grid.GetEntity(x - 1, y + 0),	
		};

		foreach (Entity neighbour in neighbours) {
			if (neighbour != null) {
				if (neighbour is Door) { return true; }
			} 
		}
		
		List<Tile> tiles = new List<Tile>() {
			grid.GetTile(x + 0, y - 1),
			grid.GetTile(x + 1, y + 0),
			grid.GetTile(x + 0, y + 1),
			grid.GetTile(x - 1, y + 0),	
		};

		foreach (Tile tile in tiles) {
			if (tile != null) {
				if (IsCorridor(tile)) { return true; }
			} 
		}
				
		return false;
	}


	public bool IsCorridor (Tile tile) {
		int x = tile.x;
		int y = tile.y;

		if (tile == null) { return false; }
		Entity up = grid.GetEntity(x, y - 1);
		Entity down = grid.GetEntity(x, y + 1);
		if (up != null && down != null && (up is Wall) && (down is Wall)) { return true; }

		Entity left = grid.GetEntity(x - 1, y);
		Entity right = grid.GetEntity(x + 1, y);
		if (left != null && right != null && (left is Wall) && (right is Wall)) { return true; }

		return false;
	}
}
