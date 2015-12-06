using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ArchitectureGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Architecture generation
	// =====================================================

	// Generate dungeon architecture (floor, wall and door tiles) 
	// for each dungeonGenerator's tree quad recursively

	public void GenerateTreeQuad (QuadTree _quadtree) {
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
						}
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						grid.CreateTile(typeof(Tile), x, y, 1, Game.assets.dungeon["floor-sandstone"]);
						Wall wall = (Wall)grid.CreateEntity(typeof(Wall), x, y, 1, Game.assets.dungeon["wall-sandstone"]) as Wall;
						wall.SetColor(new Color(0.8f, 0.8f, 0.6f));
					}
					
					// create doors
					if (dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						Door door = (Door)grid.CreateEntity(typeof(Door), x, y, 1, Game.assets.dungeon["door-closed"]) as Door;
						EntityStates[] states = new EntityStates[] { EntityStates.Open, EntityStates.Closed, EntityStates.Locked };
						door.SetState(states[Random.Range(0, states.Length)]); //Random.Range() == 0 ? EntityStates.Locked : EntityStates.Open;
					}
				}
			}
		} else {
			// Keep iterating on the quadtree
			GenerateTreeQuad(_quadtree.northWest);
			GenerateTreeQuad(_quadtree.northEast);
			GenerateTreeQuad(_quadtree.southWest);
			GenerateTreeQuad(_quadtree.southEast);
		}
	}

}
