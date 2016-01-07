using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetLoader;


public class CreatureEquipment : CreatureModule {

	// this dictionary holds the tiles used to render each equipment part on a creature

	public Dictionary<string, Tile> parts = new Dictionary<string, Tile>() {
		{ "Hair", null },
		{ "Beard", null },

		{ "Pants", null },
		{ "Boots", null },
		{ "Gloves", null },

		{ "Armour", null },
		{ "Head", null },
		{ "Hand1", null },
		{ "Hand2", null },
		{ "Cloak", null },
	};


	// =====================================================
	// Initialize Equipment Parts on Creature
	// =====================================================

	public override void Init (Creature creature) {
		base.Init(creature);

		GenerateBodyParts();
		GenerateEquipmentParts();
	}

	private void GenerateBodyParts () {
		if (me.race == "none") { return; }

		// Pants
		if (me is Player) {
			parts["Pants"] = GenerateEquipmentTile("Pants", "pants", 1, new Color(0.2f, 0.2f, 0.2f));
		}
		
		// Boots
		parts["Boots"] = GenerateEquipmentTile("Boots", "none", 2, Color.white);

		// Gloves
		parts["Gloves"] = GenerateEquipmentTile("Gloves", "none", 3,  Color.white);

		if (me is Player) {
			// hair
			string[] colors = new string[] { "#000000", "#ffff00", "#ff9900", "#ffffff", "#333333", "#A06400FF", "644600FF" };
			string hex = colors[Random.Range(0, colors.Length)];
			Color color;
			ColorUtility.TryParseHtmlString (hex, out color);
			parts["Hair"] = GenerateEquipmentTile("Hair", "hair", 4, color);

			// beard
			string[] arr =new string[] { "none", "beard" };
			string beard = me.race == "elf" ? "none" : arr[Random.Range(0, arr.Length)];
			parts["Beard"] = GenerateEquipmentTile("Beard", beard, 5, color);
		}
	}


	private void GenerateEquipmentParts () {
		if (me.race == "none") { return; }

		// equipment
		parts["Armour"] = GenerateEquipmentTile("Armour", "none", 6, Color.white);
		parts["Head"] = GenerateEquipmentTile("Head", "none", 7, Color.white);
		parts["Hand1"] = GenerateEquipmentTile("Weapon", "none", 8, Color.white);
		parts["Hand2"] = GenerateEquipmentTile("Shield", "none", 9, Color.white);
		parts["Cloak"] = GenerateEquipmentTile("Cloak", "none", -2, Color.white);
	}


	private Tile GenerateEquipmentTile (string id, string type, int zIndexPlus, Color color) {
		Transform parent = me.transform;

		Sprite asset = null;
		if (type != "none") {
			/*string path = "Tilesets/Wear/Body/" + me.race + "-" + type;
			asset = Resources.Load<Sprite>(path);
			if (asset == null) {
				Debug.LogError(path + " not found");
			}*/

			asset = Assets.GetAsset("Player/Body/" + me.race + "/" + me.race + "-" + type);
			//if (asset == null) { return; }
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

			Vector3 pos = Vector3.zero;
			Vector3 scale = tile.img.transform.localScale;
			float outlineDistance = 0.02f;

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
				/*string path = "Tilesets/Wear/Body/" + me.race + "-" + type;
				Sprite asset = Resources.Load<Sprite>(path);
				if (asset == null) {
					Debug.LogError(path + " not found");
				}*/

				Sprite asset = Assets.GetAsset("Player/Body/" + me.race + "/" + me.race + "-" + type);
				//if (asset == null) { return; }

				tile.img.sprite = asset;
				tile.img.color = item.color;
			}


			if (id == "Head") {
				if (me.race == "human" || me.race == "elf") {
					pos = new Vector3(0.015f, 0.007f, 0);
				} else if (me.race == "dwarf") {
					pos = new Vector3(-0.015f, -0.07f, 0);
				} else if (me.race == "hobbit") {
					pos = new Vector3(0.015f, -0.165f, 0);
				}

				pos += new Vector3(outlineDistance / 1, outlineDistance / 1, 0);

				tile.SetAsset(item.img.sprite);
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

				pos += new Vector3(outlineDistance / 1, outlineDistance / 1, 0);

				tile.SetAsset(item.img.sprite);
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

				pos += new Vector3(outlineDistance / 1, outlineDistance / 1, 0);

				tile.SetAsset(item.img.sprite);
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

				if (item.subtype == "Robe") {
					scale = new Vector3(scale.x, scale.y * 0.75f, scale.z); 
					pos += Vector3.up * 0.07f;
					if (me.race == "dwarf" || me.race == "hobbit") { pos -= Vector3.up * 0.04f; }
				}

				pos += new Vector3(outlineDistance / 1, outlineDistance / 1, 0);
				
				tile.SetAsset(item.img.sprite);
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

				pos += new Vector3(outlineDistance / 1, outlineDistance / 1, 0);

				// special case for loading the cloak
				/*string path = "Tilesets/Wear/Cloak/Cloak/" + AssetManager.cloakParts["Cloak"][0];
				Sprite asset = Resources.Load<Sprite>(path);
				if (asset == null) {
					Debug.LogError(path + " not found");
				}*/

				Sprite asset = Assets.GetAsset("Player/Cloak/Cloak/Cloak");// + AssetManager.cloakParts["Cloak"][0]);
				//if (asset == null) { return; }
				
				tile.SetAsset(asset);
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
