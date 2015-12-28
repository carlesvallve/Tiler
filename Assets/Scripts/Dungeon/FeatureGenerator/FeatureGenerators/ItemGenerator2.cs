using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ItemGenerator2 : DungeonFeatureGenerator {

	// =====================================================
	// Item generation
	// =====================================================

	// generate random items in dungeon level

	public override void Generate () {
		Random.seed = Dungeon.seed;
		
		// generate equipment rarity table dictionary
		int minRarity = GameData.GetDefaultEquipmentMinRarity();
		Dictionary<string, double> rarities = GameData.GenerateEquipmentRarityTable(minRarity);

		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxItems = Random.Range(0, 100) <= 80 ? Random.Range(1, Mathf.RoundToInt((room.tiles.Count * 0.2f))) : 0;

			for (int i = 1; i <= maxItems; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// Pick a weighted random item type
				System.Type itemType = Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
					{ typeof(Equipment), 	1220 },
					{ typeof(Treasure), 	80 },
					{ typeof(Food), 		40 },
					{ typeof(Potion), 		5 },
					{ typeof(Book), 		5 },
				});
				
				// get equipment id
				string id = null;
				
				if (itemType == typeof(Equipment)) {
					id = Dice.GetRandomStringFromDict(rarities);
				}

				// create item
				grid.CreateEntity(itemType, tile.x, tile.y, 0.8f, null, id);
			}
		}
	}


	// generate maxItems inside given container or creature

	public override void Generate (Tile tile, int maxItems, int minRarity = 100) {
		// generate equipment rarity table dictionary
		Dictionary<string, double> rarities = GameData.GenerateEquipmentRarityTable(minRarity);

		for (int i = 0; i < maxItems; i++) {
			System.Type itemType = tile.GetRandomItemType();

			// get item id
			string id = null;
			if (itemType == typeof(Equipment)) {
				id = Dice.GetRandomStringFromDict(rarities);
			}

			GenerateSingle(tile, itemType, id);
		}
	}


	public Item GenerateSingle (Tile tile, System.Type type, string id) {
		// create item
		Item item = (Item)grid.CreateEntity(type, 0, 0, 0.8f, null, id, false) as Item;
		
		// put the item inside the container
		item.transform.SetParent(tile.transform, false);
		item.transform.localPosition = Vector3.zero;
		item.gameObject.SetActive(false);

		// add to container's item list
		if (tile is Container) {
			((Container)tile).items.Add(item);
			return item;
		}

		// add to creature's inventory
		if (tile is Creature) {
			((Creature)tile).inventoryModule.AddItem(item);
			return item;
		}

		return item;
	}
}
