using UnityEngine;
using System.Collections;

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

		Debug.LogError("Tile could not be placed. Escaping...");
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

		Debug.LogError("Tile could not be placed. Escaping...");
		return null;
	}


	protected bool TileIsFree (Tile tile, int radius) {
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
