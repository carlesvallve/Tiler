using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ItemGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Item generation
	// =====================================================

	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxItems = Random.Range(0, 100) <= 80 ? Random.Range(1, Mathf.RoundToInt((room.tiles.Count * 0.2f))) : 0;

			for (int i = 1; i <= maxItems; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// Pick a weighted random item type
				System.Type itemType = Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
					{ typeof(Armour), 		100 },
					{ typeof(Weapon), 		100 },
					{ typeof(Shield), 		100 },

					{ typeof(Treasure), 	20 },
					{ typeof(Book), 		10 },
					{ typeof(Food), 		10 },
					{ typeof(Potion), 		10 },
				});
				
				// get item id
				string id = null;
				if (itemType == typeof(Weapon)) {
					id = GameData.weapons.ElementAt( Random.Range(0, GameData.weapons.Count)).Key;
				} else if (itemType == typeof(Armour)) {
					id = GameData.armours.ElementAt(Random.Range(0, GameData.armours.Count)).Key;
				} else if (itemType == typeof(Shield)) {
					id = GameData.shields.ElementAt(Random.Range(0, GameData.shields.Count)).Key;
				}

				// create item
				grid.CreateEntity(itemType, tile.x, tile.y, 0.8f, null, id);
			}
		}
	}

}
