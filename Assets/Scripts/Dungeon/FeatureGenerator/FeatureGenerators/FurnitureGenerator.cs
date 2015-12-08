using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FurnitureGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Furniture generation
	// =====================================================

	// TODO: 
	// Organize furniture into theme categories, and use one theme per dungeon room
	// We may want to relate this theme to the dungeon Room or TreeQuad theme


	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxFurniture = Random.Range(0, 100) <= 85 ? Random.Range(1, (int)(room.tiles.Count * 0.3f)) : 0;

			// place furniture in room
			for (int i = 1; i <= maxFurniture; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				string[] arr = new string[] { 
					"barrel-closed", "barrel-open", "barrel-water-1", "barrel-water-2", "vase",
					"bed-h", "bed-v", "chair-h", "chair-v", "throne-h", "throne-v", "stool",
					"lever-left", "lever-right", // "chest-closed", "chest-open", 
					"fountain-fire", "fountain-water", 
					"fountain-water-gold-1", "fountain-water-gold-2", "fountain-fire-gold-1", "fountain-fire-gold-2", 
					"grave-1", "grave-2", "grave-3", 
					"alarm-bell", "alarm-gong", "alarm-horn",
					"bench-h-1", "bench-h-2", "bench-v-1", "bench-v-2",
					"book-atril-h", "book-atril-v",
					"crates-1", "crates-2", "crates-3", "crates-4", "crates-5", "crates-6", "crates-7", "crates-8",
					"shelf-1","shelf-2","shelf-3","shelf-4","shelf-5","shelf-6","shelf-7","shelf-8","shelf-vertical",
					"rack-1", "rack-2", "rack-3", "rack-4",
					"table-1","table-2","table-3","table-4","table-5","table-6","table-7", "table-round-1", "table-round-2",
					"fire-1", "fire-2", "fireplace-1", "fireplace-2",
					"book-atril-h", "book-atril-v",
				};

				string path = "Tilesets/Furniture/" + arr[Random.Range(0, arr.Length)];
				Sprite asset = Resources.Load<Sprite>(path);
				if (asset == null) { Debug.LogError(path); }

				grid.CreateEntity(typeof(Furniture), tile.x, tile.y, 0.8f, asset);
			}
		}
	}

}
