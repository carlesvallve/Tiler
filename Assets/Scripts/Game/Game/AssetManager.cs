using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour {

	private static bool verbose = true;

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

	public static Dictionary <string, string[]> hand1Parts = new Dictionary <string, string[]>() {
		{ "Axe", null },
		{ "Dagger", null },
		{ "Mace", null },
		{ "Ranged", null },
		{ "Rod", null },
		{ "Spear", null },
		{ "Staff", null },
		{ "Sword", null }
	};

	public static Dictionary <string, string[]> hand2Parts = new Dictionary <string, string[]>() {
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
		SetAssetNames("Armour", armourParts);
		SetAssetNames("Head", headParts);
		SetAssetNames("Hand1", hand1Parts);
		SetAssetNames("Hand2", hand2Parts);
		SetAssetNames("Cloak", cloakParts);
	}


	private static void SetAssetNames (string part, Dictionary <string, string[]> dict) {
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


	private static string GetRandomEquipmentCategoryKey (Dictionary <string, string[]> dict) {
		List<string> keys = new List<string> (dict.Keys);
		string key = keys[Random.Range(0, keys.Count)];

		return key;
	}


	public static Sprite LoadRandomEquipmentPart (string part, Dictionary <string, string[]> dict) {
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
}
