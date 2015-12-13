using UnityEngine;
using System.Collections;

public class CreatureInventoryItem {

	public string id;
	public Item item;
	public Sprite sprite;
	public int ammount;
	public bool equipped = false;

	public CreatureInventoryItem (Item item, Sprite sprite) {
		this.item = item;
		this.sprite = sprite;
		this.id = sprite.name;
		this.ammount = item.ammount;
		this.equipped = false;
	}


	public void Equip () {
		this.equipped = true;
	}


	public void Unequip () {
		this.equipped = false;
	}
}