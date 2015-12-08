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
			int maxItems = Random.Range(0, 100) <= 90 ? Random.Range(1, Mathf.RoundToInt((room.tiles.Count * 0.3f))) : 0;

			for (int i = 1; i <= maxItems; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// Define item type classes
				List<System.Type> types = new List<System.Type>() { 
					typeof(Food), typeof(Treasure), typeof(Potion), typeof(Book), typeof(Weapon), typeof(Armour)
				};

				// Pick a random item type
				System.Type itemType = types[Random.Range(0, types.Count)];
				
				grid.CreateEntity(itemType, tile.x, tile.y, 0.8f);

			}
		}
	}

}
