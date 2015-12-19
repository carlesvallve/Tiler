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

	public virtual void Generate (Tile tile, int maxItems, int minRarity = 100) {
	}


	public virtual void GenerateAtPos (int x, int y) {
	}

	/*public virtual void GenerateAtTile<T> (T tile, int maxItems) where T : Tile {
	}*/


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


	protected Tile GetFreeTileOnRoom (DungeonRoom room, int radius = 0) {
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

		//Debug.LogWarning ("No free tile available in room " + room.id + ". Escaping...");
		return null;
	}


	protected Tile GetFreeTileOnGrid (int radius = 0) {
		// get a random free tile inside the grid
		int c = 0;

		while (true) {
			Tile tile = grid.GetTile(Random.Range(0, grid.width), Random.Range(0, grid.height));

			if (tile != null && TileIsFree(tile, radius)) {
				return tile;
			}

			c++; if (c == 1000) { break; }
		}

		//Debug.LogWarning("No free tile available in grid. Escaping...");
		return null;
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
