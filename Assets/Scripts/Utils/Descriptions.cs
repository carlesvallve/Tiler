using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Descriptions : MonoBehaviour {

	public static string GetTileDescription (Tile tile) {
		string[] arr = tile.asset.name.Split('-');
		string desc = arr[0];

		if (arr.Length > 1) {
			int n = 0;
			if (arr[1].Length > 1 && !System.Int32.TryParse(arr[1], out n)) { 
				desc = arr[1] + " " + arr[0]; 
			}
		}

		if (IsPlural(desc)) {
			desc = "some " + desc;
		} else {
			desc = StartsWithVowel(desc) ? "an " + desc : "a " + desc;
		}

		return desc;
	}


	public static string GetEquipmentDescription (Creature creature) {
		List<CreatureInventoryItem> values = new List<CreatureInventoryItem>(creature.inventoryModule.equipment.Values);

		string str = "";
		for (int i = 0; i < values.Count; i++) {
			CreatureInventoryItem invItem = values[i];
			if (invItem == null) { continue; }
			if (str == "") { 
				str += invItem.item.type == "Weapon" || invItem.item.type == "Shield" ? "(wielding " : "(wearing "; 
			}
			str += GetTileDescription(invItem.item) + ", ";
		}
		str = str.Length > 0 ? str.Substring(0, str.Length - 2) + ")" : "";

		return str;
	}


	private static bool IsPlural (string desc) {
		string[] plurals = new string[] { "gold", "bread", "meat", "water" };
		foreach (string plural in plurals) {
			if (desc == plural) { return true; }
		}

		string last = desc.Substring(desc.Length -1, 1);
		if (last == "s") { return true; }

		return false;
	}

	private static bool StartsWithVowel (string desc) {
		string first = desc.Substring(0, 1);
		if (first == "a" || first == "e" || first == "i" || first == "o" || first == "u") {
			return true;
		}

		return false;
	}
}
