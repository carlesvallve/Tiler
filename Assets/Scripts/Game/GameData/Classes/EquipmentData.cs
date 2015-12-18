using UnityEngine;

public class EquipmentData {
	public string id;
	public string[] assets;

	public string type;
	public string subtype;
	public int rarity;

	public int attack;
	public int defense;
	public string damage;
	public int armour;
	public int range;
	public int hands;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nSubtype: " + this.subtype + 
			"\nRarity: " + this.rarity + 
			"\nattack: " + this.attack + 
			"\ndefense: " + this.attack + 
			"\ndamage: "  + this.damage + 
			"\narmour: " + this.armour + 
			"\nrange: " + this.range + 
			"\nhands: " + this.hands + 
			"\nweight: " + this.weight
		);
	}
}