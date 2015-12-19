using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ItemGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Item generation
	// =====================================================

	public override void Generate () {
		// generate equipment rarity table dictionary
		Dictionary<string, double> rarities = GameData.GenerateEquipmentRarityTable();

		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxItems = Random.Range(0, 100) <= 80 ? Random.Range(1, Mathf.RoundToInt((room.tiles.Count * 0.2f))) : 0;

			for (int i = 1; i <= maxItems; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// Pick a weighted random item type
				System.Type itemType = Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
					{ typeof(Equipment), 	20 },
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


	public void GenerateInContainer (Container container, int maxItems) {
		// generate equipment rarity table dictionary
		Dictionary<string, double> rarities = GameData.GenerateEquipmentRarityTable();

		for (int i = 0; i < maxItems; i++) {
			System.Type itemType = container.GetRandomItemType();

			// get item id
			string id = null;
			if (itemType == typeof(Equipment)) {
				id = Dice.GetRandomStringFromDict(rarities);
			}

			// create item
			Item item = (Item)grid.CreateEntity(itemType, 0, 0, 0.8f, null, id, false) as Item;
			
			// put the item inside the container
			item.transform.SetParent(container.transform, false);
			item.transform.localPosition = Vector3.zero;
			item.gameObject.SetActive(false);

			container.items.Add(item);
		}
	}


	public void GenerateInCreature (Creature creature, int maxItems) {
		// generate equipment rarity table dictionary
		Dictionary<string, double> rarities = GameData.GenerateEquipmentRarityTable();

		for (int i = 0; i < maxItems; i++) {
			System.Type itemType = creature.GetRandomItemType();

			// get item id
			string id = null;
			if (itemType == typeof(Equipment)) {
				id = Dice.GetRandomStringFromDict(rarities);
			}

			// create item
			Item item = (Item)grid.CreateEntity(itemType, 0, 0, 0.8f, null, id, false) as Item;
			
			// put the item inside the container
			item.transform.SetParent(creature.transform, false);
			item.transform.localPosition = Vector3.zero;
			item.gameObject.SetActive(false);

			// add to creature's inventory
			creature.inventory.AddItem(item); // CreatureInventoryItem invItem = 

			//creature.items.Add(item);
		}
	}
}
