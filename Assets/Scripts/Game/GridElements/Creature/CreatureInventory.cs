using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureInventory  {

	protected Creature creature;
	public List<CreatureInventoryItem> items;

	/*public Dictionary<string, Item> equipment = new Dictionary<string, Item>() {
		{ "helmet",  null },
		{ "armour",  null },
		{ "shoes",   null },
		{ "weaponR", null },
		{ "weaponL", null },
		{ "ammo", 	 null }
	};*/


	public CreatureInventory (Creature creature) {
		this.creature = creature;
		items = new List<CreatureInventoryItem>();
	}


	public void AddItem (Item item) {
		Sprite sprite = item.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>().sprite;

		foreach (CreatureInventoryItem invItem in items) {
			if (sprite.name == invItem.id) {
				invItem.ammount += 1;
				return;
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


	public bool UseItem (string id) {
		foreach (CreatureInventoryItem invItem in items) {
			if (id == invItem.id) {
				Item item = invItem.item;

				if (item.CanUse(creature)) {
					item.Use(creature);
					RemoveItem(item);
					return true;
				}
				
				return false;
			}
		}

		return false;
	}
}


public class CreatureInventoryItem {

	public string id;
	public Item item;
	public Sprite sprite;
	public int ammount;
	public bool equipped;

	public CreatureInventoryItem (Item item, Sprite sprite) {
		this.item = item;
		this.sprite = sprite;
		this.id = sprite.name;
		this.ammount = 1;
		this.equipped = false;
	}


	public void Equip () {
		this.equipped = true;
	}


	public void Unequip () {
		this.equipped = false;
	}
}





