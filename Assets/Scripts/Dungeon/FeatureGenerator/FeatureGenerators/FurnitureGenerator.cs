using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetLoader;


public class FurnitureGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Furniture generation
	// =====================================================

	// TODO: 
	// Organize furniture into theme categories, and use one theme per dungeon room
	// We may want to relate this theme to the dungeon Room or TreeQuad theme


	public override void Generate (int chancePerRoom, float ratioPerFreeTiles) {
		Random.seed = Dungeon.seed;

		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxFurniture = Random.Range(0, 100) <= chancePerRoom ? Random.Range(1, (int)(room.tiles.Count * ratioPerFreeTiles)) : 0;

			// place furniture in room
			for (int i = 1; i <= maxFurniture; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { 
					//Debug.Log("Furniture could not be placed anywhere. Escaping...");
					continue; 
				}
				
				Sprite[] assets = Assets.GetCategory("Dungeon/Furniture");
				Sprite asset = assets[Random.Range(0, assets.Length)];

				grid.CreateEntity(typeof(Furniture), tile.x, tile.y, 0.8f, asset);
			}
		}
	}

}
