using UnityEngine;

public class ShieldData {
	public string id;
	public string[] assets;

	public string type;
	public string adjective;
	public int rarity;

	public int defense;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nRarity: " + this.rarity + 
			"\ndefense: " + this.defense + 
			"\nweight: " + this.weight
		);
	}
}