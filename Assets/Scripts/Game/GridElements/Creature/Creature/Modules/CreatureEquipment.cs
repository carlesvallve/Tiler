using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureEquipment : CreatureModule {

	private bool verbose = true;

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


	// =====================================================
	// Initialize Equipment Parts on Creature
	// =====================================================

	public override void Init (Creature creature) {
		base.Init(creature);

		// TODO: This should be done only once (?)
		SetParts("Armour", armourParts);
		SetParts("Head", headParts);
		SetParts("Hand1", hand1Parts);
		SetParts("Hand2", hand2Parts);
		SetParts("Cloak", cloakParts);

		GenerateBodyParts();
		GenerateEquipmentParts();
	}

	private void GenerateBodyParts () {
		if (me.race == "none") { return; }

		// Pants
		if (me is Player) {
			GenerateEquipmentTile("Pants", "pants", 1, new Color(0.2f, 0.2f, 0.2f));
		}
		
		// Boots
		GenerateEquipmentTile("Boots", "none", 2, Color.white);

		// Gloves
		GenerateEquipmentTile("Gloves", "none", 3,  Color.white);

		if (me is Player) {
			// hair
			string[] colors = new string[] { "#000000", "#ffff00", "#ff9900", "#ffffff", "#333333", "#A06400FF", "644600FF" };
			string hex = colors[Random.Range(0, colors.Length)];
			Color color;
			ColorUtility.TryParseHtmlString (hex, out color);
			GenerateEquipmentTile("Hair", "hair", 4, color);

			// beard
			string[] arr =new string[] { "none", "beard" };
			string beard = me.race == "elf" ? "none" : arr[Random.Range(0, arr.Length)];
			GenerateEquipmentTile("Beard", beard, 5, color);
		}
	}


	private void GenerateEquipmentParts () {
		if (me.race == "none") { return; }

		// equipment
		GenerateEquipmentTile("Armour", "none", 6, Color.white);
		GenerateEquipmentTile("Hat", "none", 7, Color.white);
		GenerateEquipmentTile("Weapon", "none", 8, Color.white);
		GenerateEquipmentTile("Shield", "none", 9, Color.white);
		GenerateEquipmentTile("Cloak", "none", -2, Color.white);
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
	// Get Equipment Parts FileNames
	// =====================================================

	private void SetParts (string part, Dictionary <string, string[]> dict) {
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
			
			//SpriteRenderer img = tile.img;
			//SpriteRenderer outline = tile.outline;

			Vector3 pos = Vector3.zero;
			Vector3 scale = tile.img.transform.localScale;
			float outlineDistance = 0.01f;

			if (invItem == null) { 
				tile.outline.sprite = null;
				tile.img.sprite = null;
				tile.img.color = Color.white;
				continue; 
			}

			Equipment item = (Equipment)invItem.item;

			string id = key;
			string type = item.subtype;

			// Body parts: Gloves, Boots, Pants, Hair, Beard
			if (id == "Hair" || id == "Beard" || id == "Pants" || id == "Gloves" || id == "Boots" || id == "Shoes") {
				string path = "Tilesets/Wear/Body/" + me.race + "-" + type;
				Sprite asset = Resources.Load<Sprite>(path);
				if (asset == null) {
					Debug.LogError(path + " not found");
				}

				tile.img.sprite = asset;
				tile.img.color = item.color;
			}


			if (id == "Hat") {
				if (me.race == "human" || me.race == "elf") {
					pos = new Vector3(0.015f, 0.007f, 0);
				} else if (me.race == "dwarf") {
					pos = new Vector3(-0.015f, -0.07f, 0);
				} else if (me.race == "hobbit") {
					pos = new Vector3(0.015f, -0.165f, 0);
				}

				pos += new Vector3(outlineDistance / 2, outlineDistance / 2, 0);

				tile.SetAsset(LoadRandomEquipmentPart("Head", headParts));
				tile.SetImages(scale, pos, outlineDistance);
				continue;
			}


			if (id == "Weapon") {
				if (me.race == "human" || me.race == "elf") {
					pos = new Vector3(-0.075f, 0.225f, 0);
				} else if (me.race == "dwarf") {
					pos = new Vector3(-0.05f, 0.125f, 0);
				} else if (me.race == "hobbit") {
					pos = new Vector3(0.025f, 0.2f, 0);
				}

				pos += new Vector3(outlineDistance / 2, outlineDistance / 2, 0);

				tile.SetAsset(LoadRandomEquipmentPart("Hand1", hand1Parts));
				tile.SetImages(scale, pos, outlineDistance);
				continue;
			}


			if (id == "Shield") {
				if (me.race == "human" || me.race == "elf") {
					pos = new Vector3(0f, 0.175f, 0);
				} else if (me.race == "dwarf") {
					pos = new Vector3(0f, 0.05f, 0);
				} else if (me.race == "hobbit") {
					pos = new Vector3(-0.05f, 0f, 0);
				}

				pos += new Vector3(outlineDistance / 2, outlineDistance / 2, 0);

				tile.SetAsset(LoadRandomEquipmentPart("Hand2", hand2Parts));
				tile.SetImages(scale, pos, outlineDistance);
				continue;
			}


			if (id == "Armour") {
				if (me.race == "human") {
					pos = new Vector3(0.015f, 0.075f, 0);
					scale = new Vector3(0.9f, 0.65f, 1);
				} else if (me.race == "elf") {
					pos = new Vector3(0.015f, 0.06f, 0);
					scale = new Vector3(0.9f, 0.65f, 1);
				} else if (me.race == "dwarf") {
					pos = new Vector3(-0.025f, 0.03f, 0);
					scale = new Vector3(1.1f, 0.6f, 1);
				} else if (me.race == "hobbit") {
					pos = new Vector3(0.02f, -0.025f, 0);
					scale = new Vector3(0.8f, 0.55f, 1);
				}

				pos += new Vector3(outlineDistance / 2, outlineDistance / 2, 0);
				
				tile.SetAsset(LoadRandomEquipmentPart("Armour", armourParts));
				tile.SetImages(scale, pos, outlineDistance);
				continue;
			}

			if (id == "Cloak") {
				if (me.race == "human") {
					pos = new Vector3(0.01f, 0.125f + 0.025f, 0);
					scale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "elf") {
					pos = new Vector3(0.01f, 0.125f + 0.025f, 0);
					scale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "dwarf") {
					pos = new Vector3(-0.015f, 0.05f + 0.05f, 0);
					scale = new Vector3(1.0f, 0.45f, 1);
				} else if (me.race == "hobbit") {
					pos = new Vector3(0.02f, -0.025f + 0.05f, 0);
					scale = new Vector3(0.8f, 0.35f, 1);
				}

				pos += new Vector3(outlineDistance / 2, outlineDistance / 2, 0);
				
				tile.SetAsset(LoadRandomEquipmentPart("Cloak", cloakParts));
				tile.SetImages(scale, pos, outlineDistance);
				tile.img.color = item.color;
				continue;
			}

		}
	}


	/*private Color GetRandomColor () {
		return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0, 1));
	}*/
}
