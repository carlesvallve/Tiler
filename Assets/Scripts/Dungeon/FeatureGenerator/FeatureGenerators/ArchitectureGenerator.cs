using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetLoader;


public class ArchitectureGenerator : DungeonFeatureGenerator {

	Color floorColor = new Color(0.8f, 0.8f, 0.6f);
	Color wallColor = new Color(0.8f, 0.8f, 0.6f);

	Sprite floorAsset;
	Sprite wallAsset;

	
	// =====================================================
	// Cave Architecture generation
	// =====================================================

	// Generate cave architecture (floor and wall tiles only) 
	// The whole cave will be treated as a single room

	public void GenerateCaveArchitecture () {
		SetRandomTheme();
		GenerateCave();
	}
	

	private void GenerateCave () {
		CaveGenerator cave = CaveGenerator.instance;
		int[,] Map = cave.Map;

		// set each cave cavern as a dungeon room
		DungeonGenerator.instance.rooms.Clear();

		for (int i = 0; i < cave.caverns.Count; i++) {
			DungeonRoom room = new DungeonRoom(i);
			DungeonGenerator.instance.rooms.Add(room);

			foreach (Point p in cave.caverns[i]) {
				room.tiles.Add(new DungeonTile(DungeonTileType.ROOM, p.x, p.y));
			}
		}
		
		// convert cavern to grid
		for (int y = 0; y <= Map.GetLength(1) - 1; y++) {
			for (int x = 0; x <= Map.GetLength(0) - 1; x++) {
				// create floors
				if (Map[x, y] == 0 || Map[x, y] == 2) { // 2 are supposed to be corridors painted in red

					Sprite asset = floorAsset;
					if (floorAsset == null) {
						asset = Assets.GetAsset("Dungeon/Architecture/Floor/floor-" + Random.Range(1, 5));
					}
					
					Floor floor = (Floor)grid.CreateTile(typeof(Floor), x, y, 1, asset) as Floor;
					floor.SetColor(Map[x, y] == 2 ? Color.red : floorColor, true);
					floor.roomId = 0;

					DungeonTile dtile = new DungeonTile(DungeonTileType.ROOM, x, y);
					DungeonGenerator.instance.rooms[0].tiles.Add(dtile);
				}

				// create walls
				if (Map[x, y] == 1) {
					Tile tile = grid.CreateTile(typeof(Tile), x, y, 1, null); // necessary for visibility
					tile.gameObject.name = "WallFloor";

					Sprite asset = wallAsset;
					if (wallAsset == null) {
						asset = Assets.GetAsset("Dungeon/Architecture/Wall/stone-" + Random.Range(1, 5));
					}

					Wall wall = (Wall)grid.CreateEntity(typeof(Wall), x, y, 1, asset) as Wall;
					wall.SetColor(wallColor, true);
				}
			}
		}
	}


	// =====================================================
	// Dungeon Architecture generation
	// =====================================================

	// Generate dungeon architecture (floor, wall and door tiles) 
	// for each dungeonGenerator's tree quad recursively

	public void GenerateArchitecture (QuadTree mainQuadTree) {
		SetRandomTheme();
		GenerateQuadTree(mainQuadTree);
	}


	public void GenerateQuadTree (QuadTree quadtree) {
		if (quadtree.HasChildren() == false) {

			

			for (int y = quadtree.boundary.BottomTile(); y <= quadtree.boundary.TopTile() - 1; y++) {
				for (int x = quadtree.boundary.LeftTile(); x <= quadtree.boundary.RightTile() - 1; x++) {
					// get dungeon tile on the quadtree zone
					DungeonTile dtile = dungeonGenerator.tiles[y, x];

					// In case we want to use different color in each quadtree zone...
					//Color color = quadtree.color;
					
					// create floors
					if (dtile.id == DungeonTileType.ROOM || dtile.id == DungeonTileType.CORRIDOR || 
						dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {

						Sprite asset = floorAsset;;
						if (floorAsset == null) {
							asset = Assets.GetAsset("Dungeon/Architecture/Floor/floor-" + Random.Range(1, 5));
						}
						
						Floor floor = (Floor)grid.CreateTile(typeof(Floor), x, y, 1, asset) as Floor;
						floor.SetColor(floorColor, true);

						// set room info in floor tile
						if (dtile.room != null) {
							floor.roomId = dtile.room.id;
						}
					}

					// create walls
					if (dtile.id == DungeonTileType.WALL || dtile.id == DungeonTileType.WALLCORNER) {
						grid.CreateTile(typeof(Tile), x, y, 1, null); // necessary for visibility

						Sprite asset = wallAsset;;
						if (wallAsset == null) {
							asset = Assets.GetAsset("Dungeon/Architecture/Wall/stone-" + Random.Range(1, 5));
						}

						Wall wall = (Wall)grid.CreateEntity(typeof(Wall), x, y, 1, asset) as Wall;
						wall.SetColor(wallColor, true);
					}
					
					// create doors
					if (dtile.id == DungeonTileType.DOORH || dtile.id == DungeonTileType.DOORV) {
						Door door = (Door)grid.CreateEntity(typeof(Door), x, y, 1, null) as Door;
						EntityStates[] states = new EntityStates[] { 
							EntityStates.Open, EntityStates.Closed, EntityStates.Locked 
						};
						door.SetState(states[Random.Range(0, states.Length)]);
					}
				}
			}
		} else {
			// Keep iterating on the quadtree
			GenerateQuadTree(quadtree.northWest);
			GenerateQuadTree(quadtree.northEast);
			GenerateQuadTree(quadtree.southWest);
			GenerateQuadTree(quadtree.southEast);
		}
	}


	// =====================================================
	// Theme level
	// =====================================================

	// TODO: We should probably make a Theme class and setup a lot of parameters about the theme for this whole level
	// (colors, tile types, furniture types, monster types, item types, quests, traps, etc...)

	private void SetRandomTheme () {
		Color baseColor = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
		floorColor = baseColor * 1f;
		wallColor = baseColor * 1f;

		floorAsset = Assets.GetAsset("Dungeon/Architecture/Floor/floor-gray-" + Random.Range(1, 4));
		wallAsset = Assets.GetAsset("Dungeon/Architecture/Wall/wall-gray-" + Random.Range(1, 6));
	}

}
