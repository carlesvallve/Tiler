using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour {

	private static bool verbose = false;

	// This dictionaries hold an array of equipment asset names in each category

	public static Dictionary <string, string[]> armourParts = new Dictionary <string, string[]>() {
		{ "Belt", null },
		{ "Chain", null },
		{ "Cloth", null },
		{ "HalfPlate", null },
		{ "Leather", null },
		{ "LeatherPlus", null },
		{ "Plate", null },
		{ "Robe", null }
	};

	public static Dictionary <string, string[]> headParts = new Dictionary <string, string[]>() {
		{ "Band", null },
		{ "Cap", null },
		{ "Crown", null },
		{ "Hat", null },
		{ "Helm", null },
		{ "Hood", null },
		{ "Horns", null },
		{ "Wizard", null }
	};

	public static Dictionary <string, string[]> weaponParts = new Dictionary <string, string[]>() {
		{ "Axe", null },
		{ "Dagger", null },
		{ "Mace", null },
		{ "Ranged", null },
		{ "Rod", null },
		{ "Spear", null },
		{ "Staff", null },
		{ "Sword", null }
	};

	public static Dictionary <string, string[]> shieldParts = new Dictionary <string, string[]>() {
		{ "Buckler", null },
		{ "Kite", null },
		{ "Large", null },
		{ "Round", null }
	};

	public static Dictionary <string, string[]> cloakParts = new Dictionary <string, string[]>() {
		{ "Cloak", null }
	};


	// =====================================================
	// Initialize equipment asset dictionaries
	// =====================================================

	public static void SetEquipmentAssets () {
		SetAssetNames("Armour");
		SetAssetNames("Head");
		SetAssetNames("Weapon");
		SetAssetNames("Shield");
		SetAssetNames("Cloak");
	}


	private static void SetAssetNames (string part) {
		// get dictionary keys corresponding to given part type
		Dictionary <string, string[]> dict = GetDictionaryByType(part);
		List<string> keys = new List<string> (dict.Keys);

		foreach (string key in keys) {
			string[] arr = GetFileNamesAtFolder("Tilesets/Wear/" + part + "/" + key);
			dict[key] = arr;

			if (verbose) {
				print("====== " + key + " (" + arr.Length + ") ======");
				print(ArrToString(arr));
			}
		}
	}


	private static string[] GetFileNamesAtFolder (string path) {
		Sprite[] sprites = Resources.LoadAll<Sprite>(path);

		string[] fileNames = new string[sprites.Length];
		for (int i = 0; i < sprites.Length; i++) {
			fileNames[i] = sprites[i].name;
		}

		return fileNames;
	}


	private static string ArrToString (string[] arr) {
		string str = "";
		for (int i = 0; i < arr.Length; i++) {
			str += arr[i];
			if (i < arr.Length - 1) { str += ", "; }
		}

		return str;
	}


	public static Sprite LoadEquipmentPart (string part, string key) {
		// get dictionary corresponding to given part type
		Dictionary <string, string[]> dict = GetDictionaryByType(part);

		if (dict == null) {
			Debug.LogError("No dictionary was found by " + part);
			return null;
		}

		// get path to fileName by given part key
		string[] fileNames = dict[key];
		string fileName = fileNames[Random.Range(0, fileNames.Length)];
		string path = "Tilesets/Wear/" + part + "/" + key + "/" + fileName;

		// load sprite
		Sprite asset = Resources.Load<Sprite>(path);
		if (asset == null) {
			Debug.LogError(path + " not found");
		}

		// return sprite
		// print (path + " -> " + asset.name);
		return asset;
	}


	// for debug purposes
	public static Sprite LoadRandomEquipmentPart (string part) {
		// get dictionary corresponding to given part type
		Dictionary <string, string[]> dict = GetDictionaryByType(part);

		if (dict == null) {
			Debug.LogError("No dictionary was found by " + part);
			return null;
		}

		// get filenames in category
		string key = GetRandomEquipmentCategoryKey(dict);
		string[] fileNames = dict[key];
		
		// get path to random fileName
		string fileName = fileNames[Random.Range(0, fileNames.Length)];
		string path = "Tilesets/Wear/" + part + "/" + key + "/" + fileName;

		// load sprite
		Sprite asset = Resources.Load<Sprite>(path);
		if (asset == null) {
			Debug.LogError(path + " not found");
		}

		// return sprite
		// print (path + " -> " + asset.name);
		return asset;
	}


	private static string GetRandomEquipmentCategoryKey (Dictionary <string, string[]> dict) {
		List<string> keys = new List<string> (dict.Keys);
		string key = keys[Random.Range(0, keys.Count)];

		return key;
	}


	public static Dictionary <string, string[]> GetDictionaryByType (string type) {
		switch (type) {
		case "Armour":
			return armourParts;
		case "Head":
			return headParts;
		case "Weapon":
			return weaponParts;
		case "Shield":
			return shieldParts;
		case "Cloak":
			return cloakParts;
		}

		return null;
	}
}
