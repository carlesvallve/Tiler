using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameData {

	public static Dictionary<string, MonsterData> monsters;
	public static Dictionary<string, EquipmentData> equipments;


	public void LoadAll () {
		LoadMonsters();
		LoadEquipments();

	}

	
	// =====================================================
	// Monster Data
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
			monster.subtype = 	table[y, 4];
			monster.rarity = 	int.Parse(table[y, 5]);
			
			monster.level = 	int.Parse(table[y, 6]);
			monster.hp = 		int.Parse(table[y, 7]);
			monster.movement = 	float.Parse(table[y, 8]);
			monster.attack = 	int.Parse(table[y, 9]);
			monster.defense = 	int.Parse(table[y, 10]);
			monster.damage = 	int.Parse(table[y, 11]);
			monster.armour = 	int.Parse(table[y, 12]);
			monster.vision = 	int.Parse(table[y, 13]);

			monsters.Add(id, monster);
		}

		// debug a single monster
		//monsters["Goblin"].Log();
	}


	public static Dictionary<string, double> GenerateMonsterRarityTable () {
		Dictionary<string, double> rarities = new Dictionary<string, double>();

		// iterate Gamedata.monsters dictionary and add key/rarity pairs
		foreach (KeyValuePair<string, MonsterData> entry in GameData.monsters) {
			// use entry.Value.rarity once we setup final monster spreadsheet
			// for now rarity depends on monster overall dangerousness
			
			int dlevel = Dungeon.instance.currentDungeonLevel;
			int rarity = 100 + (dlevel * 3) - ((entry.Value.hp + entry.Value.armour) * 2);

			// cap rarity so weak monsters dont appear on high dungeon levels
			int capLevel = 100;
			if (rarity > capLevel) { rarity = 0; }

			rarities.Add(entry.Key, rarity);

			//Debug.Log (entry.Key + " " + rarity + " / " + capLevel);
		}

		return rarities;
	}


	// =====================================================
	// Equipment Data
	// =====================================================

	private void LoadEquipments () {
		// load csv and generate a bidimensional table from it
		string [,] table = LoadCsv("Data/GameData/Spreadsheet - Equipment");

		// set a dictionary with all weapons
		equipments = new Dictionary<string, EquipmentData>();

		// fill each equipmentData object with data from csv table
		for (int y = 0; y < table.GetLength(0); y++) {
			string id = table[y, 0];
			if (id == "") { continue; }

			EquipmentData equipment = new EquipmentData();
			equipment.id = 		table[y, 0];
			equipment.assets = 	table[y, 1].Split(arraySeparator);
			
			equipment.type = 	table[y, 2];
			equipment.subtype = table[y, 3];
			equipment.rarity = 	int.Parse(table[y, 4]);
			
			equipment.attack = 	int.Parse(table[y, 5]);
			equipment.defense = int.Parse(table[y, 6]);
			equipment.damage = 	table[y, 7];
			equipment.armour = 	int.Parse(table[y, 8]);
			equipment.range = 	int.Parse(table[y, 9]);
			equipment.hands = 	int.Parse(table[y, 10]);
			equipment.weight = 	int.Parse(table[y, 11]);

			// debug any special kind of equipment
			//if (equipment.damage != "") { equipment.rarity *= 2; }
			//if (equipment.range > 1) { equipment.rarity *= 2; }
			if (equipment.defense > 0) { equipment.rarity *= 10; }
			if (equipment.hands > 1) { equipment.rarity *= 10; } //1060; }
			//if (equipment.armour > 0) { equipment.rarity *= 1000; }

			equipments.Add(id, equipment);
		}

		// debug a single equipment item
		//equipments["LongSword"].Log();
	}

	public static Dictionary<string, double> GenerateEquipmentRarityTable () {
		Dictionary<string, double> rarities = new Dictionary<string, double>();
		
		foreach (KeyValuePair<string, EquipmentData> entry in GameData.equipments) {
			int rarity = entry.Value.rarity;
			rarities.Add(entry.Key, rarity);
		}

		return rarities;
	}


	// =====================================================
	// Load Csv
	// =====================================================

	private static char lineSeparator = '\n';
	private static char fieldSeparator = ',';
	private static char arraySeparator = '$';

	private string [,] LoadCsv (string path, bool debug = false) {
		Debug.Log(path);

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




