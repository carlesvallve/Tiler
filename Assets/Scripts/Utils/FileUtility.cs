using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class FileUtility : MonoBehaviour {

	public Dictionary <string, string[]> bodyParts = new Dictionary <string, string[]>() {
		{ "Belt", null },
		{ "Chain", null },
		{ "Cloth", null },
		{ "HalfPlate", null },
		{ "Leather", null },
		{ "LeatherPlus", null },
		{ "Plate", null },
		{ "Robe", null }
	};

	public Dictionary <string, string[]> headParts = new Dictionary <string, string[]>() {
		{ "Band", null },
		{ "Cap", null },
		{ "Crown", null },
		{ "Hat", null },
		{ "Helm", null },
		{ "Hood", null },
		{ "Horns", null },
		{ "Wizard", null }
	};

	public Dictionary <string, string[]> hand1Parts = new Dictionary <string, string[]>() {
		{ "Axe", null },
		{ "Dagger", null },
		{ "Mace", null },
		{ "Ranged", null },
		{ "Rod", null },
		{ "Spear", null },
		{ "Staff", null },
		{ "Sword", null }
	};

	public Dictionary <string, string[]> hand2Parts = new Dictionary <string, string[]>() {
		{ "Buckler", null },
		{ "Kite", null },
		{ "Large", null },
		{ "Round", null }
	};


	void Start () {
		SetParts("Body", bodyParts);
		SetParts("Head", headParts);
		SetParts("Hand1", hand1Parts);
		SetParts("Hand2", hand2Parts);
	}


	private void SetParts (string part, Dictionary <string, string[]> dict) {
		List<string> keys = new List<string> (dict.Keys);

		foreach (string key in keys) {
			string[] arr = GetFileNamesAtFolder("Tilesets/Wear/" + part + "/" + key);
			dict[key] = arr;

			print("====== " + key + " (" + arr.Length + ") ======");
			print(ArrToString(arr));
		}
	}


	private string[] GetFileNamesAtFolder (string path) {
		Sprite[] sprites = Resources.LoadAll<Sprite>(path);

		string[] fileNames = new string[sprites.Length];
		for (int i = 0; i < sprites.Length; i++) {
			fileNames[i] = sprites[i].name;
		}

		return fileNames;
	}


	private string ArrToString (string[] arr) {
		string str = "";
		for (int i = 0; i < arr.Length; i++) {
			str += arr[i];
			if (i < arr.Length - 1) { str += ", "; }
		}

		return str;
	}
	
}
