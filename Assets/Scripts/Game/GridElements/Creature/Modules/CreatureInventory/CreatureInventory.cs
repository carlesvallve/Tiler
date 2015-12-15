using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureInventory  {
	
	protected Creature creature;

	public List<CreatureInventoryItem> items;

	public Dictionary<string, CreatureInventoryItem> equipment = 
	new Dictionary<string, CreatureInventoryItem>() {
		{ "Hat",  	null },
		{ "Cloak",  null },
		{ "Gloves", null },
		{ "Armour", null },
		{ "Weapon", null },
		{ "Ring",  	null },
		{ "Shield", null },
		{ "Boots",  null },
	};

	
	public CreatureInventory (Creature creature) {
		this.creature = creature;
		items = new List<CreatureInventoryItem>();
	}


	public void AddItem (Item item) {
		Sprite sprite = item.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>().sprite;

		if (item.stackable) {
			foreach (CreatureInventoryItem invItem in items) {
				if (sprite.name == invItem.id) {
					invItem.ammount += item.ammount;
					return;
				}
			}
		}
		
		items.Add(new CreatureInventoryItem(item, sprite));
	}


	public void RemoveItem (Item item) {
		Sprite sprite = item.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>().sprite;

		foreach (CreatureInventoryItem invItem in items) {
			if (sprite.name == invItem.id) {
				if (invItem.ammount > 1) {
					invItem.ammount -= 1;
				} else {
					items.Remove(invItem);
				}
				return;
			} 
		}
	}


	public CreatureInventoryItem GetInventoryItemById (string id) {
		foreach (CreatureInventoryItem invItem in items) {
			if (id == invItem.id) {
				return invItem;
			}
		}

		return null;
	}


	public bool UseItem (CreatureInventoryItem invItem) {
		Item item = invItem.item;

		if (item.CanUse(creature)) {
			item.Use(creature);
			RemoveItem(item);
			return true;
		} 

		return false;
	}


	public void EquipItem (CreatureInventoryItem invItem) {
		Item item = invItem.item;
		if (item.equipmentSlot == null) { return; }

		// unequipped equipped item
		if (equipment[item.equipmentSlot] != null) {
			bool wasEquipped = invItem.equipped;
			equipment[item.equipmentSlot].equipped = false;
			equipment[item.equipmentSlot] = null;

			if (wasEquipped) { 
				creature.UpdateEquipmentStats(); //SetWeapons();
				return; 
			}
		}
		
		// equip non-equipped item
		if (!invItem.equipped) {
			equipment[item.equipmentSlot] = invItem;
			invItem.equipped = true;

			//creature.SetWeapons();
			creature.UpdateEquipmentStats(); //SetWeapons();

			return;
		}
	}

}







