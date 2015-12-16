using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameData : MonoBehaviour {

	private static char lineSeparator = '\n';
	private static char fieldSeparator = ',';
	private static char arraySeparator = '$';

	void Start() {
		//Text text = GameObject.Find("Text").GetComponent<Text>();
		//text.text = CsvReader.Load("Data/GameData/Monsters");

		//LoadMonsters();
		LoadArmours();
		//LoadWeapons();
	}


	private string [,] LoadCsv (string path, bool debug = true) {
		TextAsset csvFile = Resources.Load(path) as TextAsset;

		print (csvFile);

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


	
	public static Dictionary<string, MonsterData> monsters;
	public static Dictionary<string, ArmourData> armours;
	public static Dictionary<string, WeaponData> weapons;
	
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
			
			monster.race = 		table[y, 2];
			monster.type = 		table[y, 3];
			monster.adjective = table[y, 4];
			
			monster.level = 	int.Parse(table[y, 5]);
			monster.movement = 	float.Parse(table[y, 6]);
			monster.attack = 	int.Parse(table[y, 7]);
			monster.defense = 	int.Parse(table[y, 8]);
			monster.damage = 	int.Parse(table[y, 9]);
			monster.vision = 	int.Parse(table[y, 10]);

			monsters.Add(id, monster);
		}

		// debug a single monster
		monsters["Goblin"].Log();
		
	}


	private void LoadArmours () {
		// load csv and generate a bidimensional table from it
		string [,] table = LoadCsv("Data/GameData/Spreadsheet - Armour");

		// set a dictionary with all monsters
		armours = new Dictionary<string, ArmourData>();

		// fill each armourData object with data from csv table
		for (int y = 0; y < table.GetLength(0); y++) {
			string id = table[y, 0];
			print (">>>>>>>>>>>>" + id);
			if (id == "") { continue; }

			ArmourData armour = new ArmourData();
			armour.id = 			table[y, 0];
			armour.assets = 		table[y, 1].Split(arraySeparator);
			
			armour.type = 			table[y, 2];
			armour.adjective = 		table[y, 3];
			
			armour.ac = 			int.Parse(table[y, 4]);
			armour.grd = 			int.Parse(table[y, 5]);
			armour.sh = 			int.Parse(table[y, 6]);
			armour.ev = 			int.Parse(table[y, 7]);
			armour.encumberness = 	int.Parse(table[y, 8]);
			armour.weight = 		int.Parse(table[y, 9]);

			armours.Add(id, armour);
		}

		// debug a single monster
		armours["LeatherArmour"].Log();
		
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

		// debug a single monster
		weapons["LongSword"].Log();
		
	}
}


public class MonsterData {
	public string id;
	public string[] assets;

	public string race;
	public string type;
	public string adjective;

	public int level;
	public float movement;
	public int attack;
	public int defense;
	public int damage;
	public int vision;


	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nRace: " + this.race + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nLevel: " + this.level + 
			"\nMovement: "  + this.movement + 
			"\nAttack: " + this.attack + 
			"\nDefense: " + this.defense + 
			"\nDamage: " + this.damage + 
			"\nVision: " + this.vision
		);
	}
}


public class ArmourData {
	public string id;
	public string[] assets;

	public string type;
	public string adjective;

	public int ac;
	public int grd;
	public int sh;
	public int ev;
	public int encumberness;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nac: " + this.ac + 
			"\ngrd: "  + this.grd + 
			"\nsh: " + this.sh + 
			"\nev: " + this.ev + 
			"\nencumberness: " + this.encumberness + 
			"\nweight: " + this.weight
		);
	}
}


public class WeaponData {
	public string id;
	public string[] assets;

	public string type;
	public string adjective;

	public int hit;
	public string damage;
	public int range;
	public int hands;
	public int weight;

	public void Log () {
		Debug.Log("Id: " + this.id + 
			"\nassets: " + this.assets.Length + 
			"\nType: " + this.type + 
			"\nAdjective: " + this.adjective + 
			"\nhit: " + this.hit + 
			"\ndamage: "  + this.damage + 
			"\nrange: " + this.range + 
			"\nhands: " + this.hands + 
			"\nweight: " + this.weight
		);
	}
}




