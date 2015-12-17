using UnityEngine;

public class ArmourData {
	public string id;
	public string[] assets;

	public string type;
	public string adjective;
	public int rarity;

	public int ac;
	public int gdr;
	public int sh;
	public int ev;
	public int encumberness;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nRarity: " + this.rarity + 
			"\nac: " + this.ac + 
			"\ngdr: "  + this.gdr + 
			"\nsh: " + this.sh + 
			"\nev: " + this.ev + 
			"\nencumberness: " + this.encumberness + 
			"\nweight: " + this.weight
		);
	}
}
