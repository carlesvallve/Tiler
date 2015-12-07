using UnityEngine;
using System.Collections;

public class CreatureStats {
	public int level = 1;
	public int xp = 0;

	// hp
	public int hpMax = 10;
	public int hp = 10;

	// alert
	public int alert = 0;
	public int alertMax = 2;

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

	// regeneration
	public float regeneration = 0;
	public float regenerationRate = 0.1f; // 1 point each 10 turns

	// hunger
	public float hunger = 0;
	public float hungerRate = 0.1f; // 1 point each 10 turns

	// vision
	public int visionRadius = 4;
	public int awareness = 1;
	public int stealth = 1;

	// gold
	public int gold = 0;
}
