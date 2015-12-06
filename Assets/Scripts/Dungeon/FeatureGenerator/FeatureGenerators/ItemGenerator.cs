using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ItemGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Item generation
	// =====================================================

	// TODO: 
	// Organize items into sub-categories: Food, Treasure, Potion, Book, Weapon, Armour...
	// Items should be spawned depending on the overall level difficulty and balance


	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxItems = Random.Range(0, 100) <= 50 ? Random.Range(1, (int)(room.tiles.Count * 0.3f)) : 0;

			// place items in room
			for (int i = 1; i <= maxItems; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				string[] arr = new string[] { 
					"apricot", "banana", "bread", "meat", "strawberry" 
				};

				Sprite asset = Game.assets.item[arr[Random.Range(0, arr.Length)]];
				grid.CreateEntity(typeof(Item), tile.x, tile.y, 0.8f, asset);
			}
		}
	}

}
