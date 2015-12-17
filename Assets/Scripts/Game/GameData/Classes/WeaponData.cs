﻿using UnityEngine;

public class WeaponData {
	public string id;
	public string[] assets;

	public string type;
	public string adjective;
	public int rarity;

	public int attack;
	public string damage;
	public int range;
	public int hands;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nRarity: " + this.rarity + 
			"\nattack: " + this.attack + 
			"\ndamage: "  + this.damage + 
			"\nrange: " + this.range + 
			"\nhands: " + this.hands + 
			"\nweight: " + this.weight
		);
	}
}