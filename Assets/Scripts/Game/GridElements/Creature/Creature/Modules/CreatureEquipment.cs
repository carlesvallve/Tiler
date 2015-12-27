using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureEquipment : CreatureModule {

	/*public Dictionary <string, string[]> bodyParts = new Dictionary <string, string[]>() {
		{ "Body", null },
		{ "Beard", null },
		{ "Boots", null },
		{ "Gloves", null },
		{ "Hair", null },
		{ "Pants", null },
		{ "Shoes", null }
	};*/

	public Dictionary <string, string[]> armourParts = new Dictionary <string, string[]>() {
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

	public Dictionary <string, string[]> cloakParts = new Dictionary <string, string[]>() {
		{ "Cloak", null }
	};


	public override void Init (Creature creature) {
		base.Init(creature);

		SetParts("Armour", armourParts);
		SetParts("Head", headParts);
		SetParts("Hand1", hand1Parts);
		SetParts("Hand2", hand2Parts);
		SetParts("Cloak", cloakParts);

		Initialize();
	}


	// =====================================================
	// Get Equipment Parts FileNames
	// =====================================================

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


	private string GetRandomEquipmentCategoryKey (Dictionary <string, string[]> dict) {
		List<string> keys = new List<string> (dict.Keys);
		string key = keys[Random.Range(0, keys.Count)];

		return key;
	}


	private Sprite LoadRandomEquipmentPart (string part, Dictionary <string, string[]> dict) {
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


	// =====================================================
	// Initialize Equipment Parts on Creature
	// =====================================================


	private void Initialize () {
		if (me.race == "none") {
			return;
		}

		// Pants / boots / gloves
		GenerateEquipmentTile("Pants", "pants", 0, new Color(0.2f, 0.2f, 0.2f));
		GenerateEquipmentTile("Boots", "none", 1, Color.white);
		GenerateEquipmentTile("Gloves", "none", 2,  Color.white);

		// hair
		string[] colors = new string[] { "#000000", "#ffff00", "#ff9900", "#ffffff", "#333333", "#A06400FF", "644600FF" };
		string hex = colors[Random.Range(0, colors.Length)];
		Color color;
		ColorUtility.TryParseHtmlString (hex, out color);
		GenerateEquipmentTile("Hair", "hair", 3, color);

		// beard
		string[] arr =new string[] { "none", "beard" };
		string beard = me.race == "elf" ? "none" : arr[Random.Range(0, arr.Length)];
		GenerateEquipmentTile("Beard", beard, 4, color);

		// equipment
		GenerateEquipmentTile("Armour", "none", 5, Color.white);
		GenerateEquipmentTile("Hat", "none", 6, Color.white);
		GenerateEquipmentTile("Weapon", "none", 7, Color.white);
		GenerateEquipmentTile("Shield", "none", 8, Color.white);
		GenerateEquipmentTile("Cloak", "none", -1, Color.white);
	}


	private Tile GenerateEquipmentTile (string id, string type, int zIndexPlus, Color color) {
		Transform parent = me.transform;

		Sprite asset = null;
		if (type != "none") {
			string path = "Tilesets/Wear/Body/" + me.race + "-" + type;
			asset = Resources.Load<Sprite>(path);
			if (asset == null) {
				Debug.LogError(path + " not found");
			}
		}
		
		GameObject obj = (GameObject)Instantiate(grid.tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = id;

		Tile tile = obj.AddComponent<Tile>();
		tile.Init(grid, me.x, me.y, me.scale, asset, null);

		obj.transform.localPosition = Vector3.zero;

		tile.zIndex = me.zIndex + zIndexPlus;
		tile.SetSortingOrder();

		SpriteRenderer img = tile.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>();
		img.color = color;
		img.transform.localPosition = new Vector3(-0.035f, 0.135f, 0);

		return tile;
	}


	// =====================================================
	// Render Equipment Parts on Creature
	// =====================================================

	public void Render () {
		if (me.race == "none") {
			return;
		}

		List<string> keys = new List<string>(me.inventoryModule.equipment.Keys);
		List<CreatureInventoryItem> values = new List<CreatureInventoryItem>(me.inventoryModule.equipment.Values);

		for (int i = 0; i < keys.Count; i++) {
			string key = keys[i];
			CreatureInventoryItem invItem = values[i];

			Transform tr = me.transform.Find(key);
			if (tr == null) { continue; }
			Tile tile = tr.GetComponent<Tile>();
			
			SpriteRenderer img = tile.img;

			if (invItem == null) { 
				img.sprite = null;
				img.color = Color.white;
				continue; 
			}

			Equipment item = (Equipment)invItem.item;

			string id = key;
			string type = item.subtype;

			// Gloves, Boots, Pants, Hair, Beard
			if (id == "Hair" || id == "Beard" || id == "Pants" || id == "Gloves" || id == "Boots" || id == "Shoes") {
				string path = "Tilesets/Wear/Body/" + me.race + "-" + type;
				Sprite asset = Resources.Load<Sprite>(path);
				if (asset == null) {
					Debug.LogError(path + " not found");
				}

				img.sprite = asset;
				img.color = item.color;
			}


			if (id == "Hat") {
				img.sprite = LoadRandomEquipmentPart("Head", headParts);

				if (me.race == "human" || me.race == "elf") {
					img.transform.localPosition = new Vector3(0.015f, 0.0075f, 0);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(0.015f, -0.075f, 0);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.015f, -0.165f, 0);
				}

				continue;
			}


			if (id == "Weapon") {
				img.sprite = LoadRandomEquipmentPart("Hand1", hand1Parts);
				
				if (me.race == "human" || me.race == "elf") {
					img.transform.localPosition = new Vector3(-0.075f, 0.225f, 0);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(-0.05f, 0.125f, 0);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.025f, 0.2f, 0);
				}

				continue;
			}


			if (id == "Shield") {
				img.sprite = LoadRandomEquipmentPart("Hand2", hand2Parts);
				

				if (me.race == "human" || me.race == "elf") {
					img.transform.localPosition = new Vector3(0f, 0.175f, 0);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(0f, 0.05f, 0);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(-0.05f, 0f, 0);
				}

				continue;
			}


			if (id == "Armour") {
				img.sprite = LoadRandomEquipmentPart("Armour", armourParts);

				if (me.race == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.1f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.65f, 1);
				} else if (me.race == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.1f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.65f, 1);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(-0.015f, 0.05f, 0);
					img.transform.localScale = new Vector3(1.2f, 0.6f, 1);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.02f, -0.025f, 0);
					img.transform.localScale = new Vector3(0.8f, 0.55f, 1);
				}
				
				continue;
			}

			if (id == "Cloak") {
				img.sprite = LoadRandomEquipmentPart("Cloak", cloakParts);
				img.color = item.color;
				
				if (me.race == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.025f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.025f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(-0.015f, 0.05f + 0.05f, 0);
					img.transform.localScale = new Vector3(1.0f, 0.45f, 1);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.02f, -0.025f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.8f, 0.35f, 1);
				}
				
				continue;
			}

		}
	}


	/*private Color GetRandomColor () {
		return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0, 1));
	}*/
}
