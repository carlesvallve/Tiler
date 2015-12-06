using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FurnitureGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Furniture generation
	// =====================================================

	public override void Generate () {
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

				Sprite asset = Game.assets.dungeon[arr[Random.Range(0, arr.Length)]];
				grid.CreateEntity(typeof(Furniture), tile.x, tile.y, 0.8f, asset);
			}
		}
	}

}
