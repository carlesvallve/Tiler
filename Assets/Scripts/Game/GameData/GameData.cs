using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameData {

	public static Dictionary<string, MonsterData> monsters;
	public static Dictionary<string, WeaponData> weapons;
	public static Dictionary<string, ArmourData> armours;


	public void LoadAll () {
		LoadMonsters();
		LoadWeapons();
		LoadArmours();
	}

	
	// =====================================================
	// Parse Monsters, Weapons, Armour
	// =====================================================

	private void LoadMonsters () {
		// load csv and generate a bidimensional table from it
		string [,] table = LoadCsv("Data/GameData/Spreadsheet - Monsters");

		// set a dictionary with all monsters
		monsters = new Dictionary<string, MonsterData>();

		// fill each monsterData object with data from csv table
		for (int y = 0; y < table.GetLength(0); y++) {
			string id = table[y, 0];
			if (id == "") { continue; }

			MonsterData monster = new MonsterData();
			monster.id = 		table[y, 0];
			monster.assets = 	table[y, 1].Split(arraySeparator);
			monster.type = 		table[y, 2];
			monster.race = 		table[y, 3];
			monster.adjective = table[y, 4];
			monster.level = 	int.Parse(table[y, 5]);
			monster.hp = 		int.Parse(table[y, 6]);
			monster.movement = 	float.Parse(table[y, 7]);
			monster.attack = 	int.Parse(table[y, 8]);
			monster.defense = 	int.Parse(table[y, 9]);
			monster.damage = 	int.Parse(table[y, 10]);
			monster.gdr = 		int.Parse(table[y, 11]);
			monster.vision = 	int.Parse(table[y, 12]);

			monsters.Add(id, monster);
		}

		// debug a single monster
		//monsters["Goblin"].Log();
		
	}


	private void LoadWeapons () {
		// load csv and generate a bidimensional table from it
		string [,] table = LoadCsv("Data/GameData/Spreadsheet - Weapons");

		// set a dictionary with all weapons
		weapons = new Dictionary<string,WeaponData>();

		// fill each weaponData object with data from csv table
		for (int y = 0; y < table.GetLength(0); y++) {
			string id = table[y, 0];
			if (id == "") { continue; }

			WeaponData weapon = new WeaponData();
			weapon.id = 		table[y, 0];
			weapon.assets = 	table[y, 1].Split(arraySeparator);
			weapon.type = 		table[y, 2];
			weapon.adjective = 	table[y, 3];
			weapon.hit = 		int.Parse(table[y, 4]);
			weapon.damage = 	table[y, 5];
			weapon.range = 		int.Parse(table[y, 6]);
			weapon.hands = 		int.Parse(table[y, 7]);
			weapon.weight = 	int.Parse(table[y, 8]);

			weapons.Add(id, weapon);
		}

		// debug a single weapon
		//weapons["LongSword"].Log();
		
	}


	private void LoadArmours () {
		// load csv and generate a bidimensional table from it
		string [,] table = LoadCsv("Data/GameData/Spreadsheet - Armour");

		// set a dictionary with all monsters
		armours = new Dictionary<string, ArmourData>();

		// fill each armourData object with data from csv table
		for (int y = 0; y < table.GetLength(0); y++) {
			string id = table[y, 0];
			if (id == "") { continue; }

			ArmourData armour = new ArmourData();
			armour.id = 			table[y, 0];
			armour.assets = 		table[y, 1].Split(arraySeparator);
			armour.type = 			table[y, 2];
			armour.adjective = 		table[y, 3];
			armour.ac = 			int.Parse(table[y, 4]);
			armour.gdr = 			int.Parse(table[y, 5]);
			armour.sh = 			int.Parse(table[y, 6]);
			armour.ev = 			int.Parse(table[y, 7]);
			armour.encumberness = 	int.Parse(table[y, 8]);
			armour.weight = 		int.Parse(table[y, 9]);

			armours.Add(id, armour);
		}

		// debug a single armour
		//armours["LeatherArmour"].Log();
	}


	// =====================================================
	// Load Csv
	// =====================================================

	private static char lineSeparator = '\n';
	private static char fieldSeparator = ',';
	private static char arraySeparator = '$';

	private string [,] LoadCsv (string path, bool debug = false) {
		TextAsset csvFile = Resources.Load(path) as TextAsset;

		string[] records = csvFile.text.Split (lineSeparator);
		string[] headers = records[0].Split(fieldSeparator);
		string[,] table = new string[records.Length - 1, headers.Length];

		for (int y = 1; y < records.Length; y++) {
			string[] fields = records[y].Split(fieldSeparator);

			for (int x = 0; x < fields.Length; x++) {
				table[y-1, x] = fields[x];
			}
		}

		if (debug) {
			// debug headers
			string str = "";
			for (int x = 0; x < headers.Length; x++) {
				str += headers[x]; if (x < headers.Length) { str += ", "; }
			}
			Debug.Log(str);

			// debug table
			for (int y = 0; y < table.GetLength(0); y++) {
				str = "";
				for (int x = 0; x < table.GetLength(1); x++) {
					str += table[y, x]; if (x < table.GetLength(1)) { str += ", "; }
				}
				Debug.Log(str);
			}
		}
		
		return table;
	}
}




