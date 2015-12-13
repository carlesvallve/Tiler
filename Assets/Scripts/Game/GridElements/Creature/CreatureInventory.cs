using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureInventory  {

	/*public Dictionary<string, List<Item>> inventory = new Dictionary<string, List<Item>>() {
		{ "food",     new List<Item>() },
		{ "treasure", new List<Item>() },
		{ "potion",   new List<Item>() },
		{ "book", 	  new List<Item>() },
		{ "weapon",   new List<Item>() },
		{ "armour",   new List<Item>() }
	};*/


	public List<CreatureInventoryItem> items;


	public CreatureInventory () {
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

}


public class CreatureInventoryItem {

	public string id;
	public Item item;
	public Sprite sprite;
	public int ammount;

	public CreatureInventoryItem (Item item, Sprite sprite) {
		this.item = item;
		this.sprite = sprite;
		this.id = sprite.name;
		this.ammount = 1;
	}
}





