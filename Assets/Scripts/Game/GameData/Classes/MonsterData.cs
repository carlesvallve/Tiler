using UnityEngine;

public class MonsterData {
	public string id;
	public string[] assets;

	public string race;
	public string type;
	public string adjective;

	public int level;
	public int hp;
	public float movement;
	public int attack;
	public int defense;
	public int damage;
	public int gdr;
	public int vision;


	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nRace: " + this.race + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nLevel: " + this.level + 
			"\nHp: "  + this.hp + 
			"\nMovement: "  + this.movement + 
			"\nAttack: " + this.attack + 
			"\nDefense: " + this.defense + 
			"\nDamage: " + this.damage + 
			"\nGdr: " + this.gdr + 
			"\nVision: " + this.vision
		);
	}
}