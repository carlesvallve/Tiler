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


	public CreatureInventoryItem AddItem (Item item) {
		Sprite sprite = item.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>().sprite;

		if (item.stackable) {
			foreach (CreatureInventoryItem invItm in items) {
				if (sprite.name == invItm.id) {
					invItm.ammount += item.ammount;
					return invItm;
				}
			}
		}
		
		CreatureInventoryItem invItem = new CreatureInventoryItem(item, sprite);
		items.Add(invItem);

		return invItem;
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
				creature.UpdateEquipmentStats();
				return; 
			}
		}
		
		// equip non-equipped item
		if (!invItem.equipped) {
			equipment[item.equipmentSlot] = invItem;
			invItem.equipped = true;
		}

		// handle special cases
		Equipment eq = (Equipment)invItem.item;

		// equipping a 2 hand weapon and removes equipped shield
		if (eq.hands == 2) {
			if (equipment["Shield"] != null) {
				equipment["Shield"].equipped = false;
				equipment["Shield"] = null;
			}
			
		// equipping a shield removes equipped 2 hand weapon
		} else if (eq.equipmentSlot == "Shield") {
			if (equipment["Weapon"] != null && ((Equipment)equipment["Weapon"].item).hands == 2) {
				equipment["Weapon"].equipped = false;
				equipment["Weapon"] = null;
			}
		}

		// update creature's equipment stats
		creature.UpdateEquipmentStats();
	}


	public void UnequipItem (CreatureInventoryItem invItem) {

	}


	public bool IsBestItem (CreatureInventoryItem invItem) {
		if (!(invItem.item is Equipment)) {
			return false;
		}

		// get equippable item and slot
		Equipment item = (Equipment)invItem.item;
		string slot = item.equipmentSlot;

		// automatically equip if we are wearing nothing on item's slot
		if (this.equipment[slot] == null) {
			return true;
		}

		// iterate all items
		foreach (CreatureInventoryItem invItm in this.items) {
			if (invItem == invItm) { continue; }
			if (!(invItm.item is Equipment)) { continue; }

			// compare only with items of same slot type
			Equipment itm = (Equipment)invItm.item;
			if (slot != itm.equipmentSlot) { continue; }

			//Debug.Log (item.name + " " + item.armour + " / " + itm.name + " " + itm.armour);
			
			// better weapon
			if (item.damage != "" && Dice.GetMaxValue(item.damage) > Dice.GetMaxValue(itm.damage)) { 
				return true; 
			}
			
			// better armour
			if (item.armour > 0 && item.armour > itm.armour) { 
				return true; 
			}

			// better shield
			if (item.defense > 0 && item.defense > itm.defense) { 
				return true; 
			}
		}
		
		return false;
	}

}







