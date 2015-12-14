﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureStats {

	// speed
	public float energy = 1f;
	public float energyRate = 1f;

	// xp
	public int level = 1;
	public int xp = 0;
	public int xpMax = 100;
	public int xpValue = 20;

	// hp
	public int hpMax = 10;
	public int hp = 10;

	// vision
	public int visionRadius = 5;
	public int alert = 0;
	public int alertMax = 4;

	// basic stats
	public int str = 1;
	public int dex = 1;
	public int con = 1;
	public int intel = 1;
	public int wis = 1;
	public int cha = 1;

	// combat
	public int attack = 1;
	public int defense = 1;

	public Weapon weapon = null;
	public Shield shield = null;

	public int attackRange = 1;

	// regeneration
	public float regeneration = 0;
	public float regenerationRate = 0.2f; // 1 point each 5 turns

	// hunger (todo)
	public float hunger = 0;
	public float hungerRate = 0.1f; // 10 point each 5 turns

	// stealth (todo)
	public int awareness = 1;
	public int stealth = 1;

	// gold
	public int gold = 0;

	// ai interest

	public Dictionary<System.Type, int> interest = new Dictionary<System.Type, int>();
	public Dictionary<System.Type, int> baseInterest = new Dictionary<System.Type, int>() {
		// greed for items
		{ typeof(Armour), 10 },
		{ typeof(Weapon), 10 },
		{ typeof(Book), 10 },
		{ typeof(Food), 10 },
		{ typeof(Potion), 10 },
		{ typeof(Treasure), 10 },

		// hate/fear from monsters
		{ typeof(Player), 100 },
		{ typeof(Monster), 0 },
	};


	public CreatureStats () {
		// fill interest dict with baseIterest values
		foreach (System.Type key in interest.Keys) {
     		interest.Add(key, baseInterest[key]);
 		}
	}

}
