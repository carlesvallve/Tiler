using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureEquipment : CreatureModule {


	public override void Init (Creature creature) {
		base.Init(creature);

		Initialize();
	}


	private void Initialize () {
		if (me.race == "none") {
			return;
		}

		// Pants / boots / gloves
		GenerateEquipmentTile("Pants", "pants", 0, Color.gray);
		GenerateEquipmentTile("Boots", "none", 1, Color.white); // GetRandomColor()
		GenerateEquipmentTile("Gloves", "none", 2,  Color.white); //GetRandomColor());

		// hair
		string[] colors = new string[] { "#000000", "#ffff00", "#ff9900", "#ffffff", "#333333", "#A06400FF", "644600FF" };
		string hex = colors[Random.Range(0, colors.Length)];
		Color color = Color.red;
		ColorUtility.TryParseHtmlString (hex, out color);
		GenerateEquipmentTile("Hair", "hair", 3, color);

		// beard
		string[] arr =new string[] { "none", "beard" };
		string beard = me.race == "elf" ? "none" : arr[Random.Range(0, arr.Length)];
		GenerateEquipmentTile("Beard", beard, 4, color);

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
			string path = "Tilesets/Basic/" + me.race + "-" + type;
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


			if (id == "Hat") {
				
				string[] sprites = new string[] {
					"full_gold", "band_red", "band_blue", "bandana_ybrown", "black_horn", 
					"brown_gold", "cap_black1", "chain", "crown_gold1", "feather_green",
					"fhelm_gray3", "fhelm_horn_yellow", "feather_white", "helm_plume", "hood_red", "horned", "iron1",
					"iron2", "isildur", "ninja_black", "viking_brown1", "wizard_purple", "yellow_wing",
					"black_horn2", "brown_gold", "cheek_red", "fhelm_horn2", "full_black", "gandalf", "hat_black", 
					"healer", "helm_gimli", "helm_green", "helm_red", "hood_black2", "hood_gray", "hood_green", 
					"hood_orange", "hood_white", "hood_white2", "horn_evil", "horns1", "horns2", "iron3", 
					"viking_brown2", "viking_gold", "wizard_blackgold", "wizard_brown" 
				};
				string spriteName = sprites[Random.Range(0, sprites.Length)];
				string path4 = "Tilesets/Player/head/" + spriteName;
				Sprite asset4 = Resources.Load<Sprite>(path4);
				if (asset4 == null) {
					Debug.LogError(path4 + " not found");
				}

				img.sprite = asset4;
				img.color = Color.white;

				if (me.race == "human" || me.race == "elf") {
					img.transform.localPosition = new Vector3(0.015f, 0.01f, 0);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(0.015f, -0.075f, 0);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.015f, -0.165f, 0);
				}

				continue;
			}


			if (id == "Weapon") {
				
				string[] sprites = new string[] {
					"aragorn", "arwen", "axe", "blessed_blade", 
					"bow", "club_slant", "dagger", "frodo", "gandalf",
					"giant_club_plain", "gimli", "katana", "lance", "mace", "quarterstaff1", "short_sword",
					"trident", "war_axe", "sword3", "staff_evil", "spear1", "long_sword_slant", "boromir", 
					"axe_blood", "black_whip", "club3", "crossbow", "crossbow3", "dagger_slant", "enchantress_dagger", 
					"eveningstar", "falchion", "fork2", "flail_ball", "glaive", "great_axe", "great_bow", "great_sword", 
					"hand_axe", "heavy_sword", "hook", "katana_slant", "lance2", "legolas", "morningstar", "scimitar"
				};
				string spriteName = sprites[Random.Range(0, sprites.Length)];
				string path3 = "Tilesets/Player/hand1/" + spriteName;
				Sprite asset3 = Resources.Load<Sprite>(path3);
				if (asset3 == null) {
					Debug.LogError(path3 + " not found");
				}

				img.sprite = asset3;
				img.color = Color.white;

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
				
				string[] sprites = new string[] {
					"boromir", "bullseye", "shield_kite1", "shield_kite4", 
					"shield_knight_blue", "shield_knight_rw", "Shield_long_cross", "shield_middle_black", "gong",
					"shield_middle_round", "shield_middle_unicorn", "shield_round5", "shield_skull", "shield_round_small", 
					"shield_shaman", "gil-galad", "shield_middle_ethn", "shield_round1", "shield_round_white", 
					"shield_middle_gray", "shield_middle_brown", "shield_middle_black"
				};
				string spriteName = sprites[Random.Range(0, sprites.Length)];
				string path5 = "Tilesets/Player/hand2/" + spriteName;
				Sprite asset5 = Resources.Load<Sprite>(path5);
				if (asset5 == null) {
					Debug.LogError(path5 + " not found");
				}

				img.sprite = asset5;
				img.color = Color.white;

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
				
				string[] sprites = new string[] {
					"leather_armour", "leather_green", "leather_metal", "leather_red", 
					"karate", "jessica", "mesh_black", "legolas", "metal_blue",
					"leather_heavy", "plate", "plate2", "pj", "ringmail", "robe_black", "animal_skin",
					"aragorn", "aragorn2", "armor_mummy", "arwen", "banded", "banded2", "belt1", "belt2", 
					"bikini_red", "bloody", "boromir", "bplate_green", "bplate_metal1", "breast_black", "chainmail", "chainmail3", 
					"china_red2", "chunli", "coat_black", "dragonsc_gold", "dress_white", "half_plate3", "isildur", "jacket3", 
					"jacket_stud", "karate2", "lears_chain_mail", "leather_armour3", "leather_stud", "legolas", "merry", 
					"mesh_red", "monk_black", "neck", "plate_and_cloth", "plate_and_cloth2", "plate_black", "robe_black_gold", 
					"robe_brown3", "robe_gray2", "robe_misfortune", "robe_of_night", "robe_white2", "scalemail", "shirt_black_and_cloth"  
				};
				string spriteName = sprites[Random.Range(0, sprites.Length)];
				string path2 = "Tilesets/Player/body/" + spriteName;
				Sprite asset2 = Resources.Load<Sprite>(path2);
				if (asset2 == null) {
					Debug.LogError(path2 + " not found");
				}

				img.sprite = asset2;
				img.color = Color.white;

				if (me.race == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.12f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.7f, 1);
				} else if (me.race == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.12f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.7f, 1);
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
				
				string[] sprites = new string[] { "white" };
				string spriteName = sprites[Random.Range(0, sprites.Length)];
				string path6 = "Tilesets/Player/cloak/" + spriteName;
				Sprite asset6 = Resources.Load<Sprite>(path6);
				if (asset6 == null) {
					Debug.LogError(path6 + " not found");
				}

				img.sprite = asset6;
				img.color = GetRandomColor();

				if (me.race == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (me.race == "dwarf") {
					img.transform.localPosition = new Vector3(-0.015f, 0.05f + 0.05f, 0);
					img.transform.localScale = new Vector3(1.0f, 0.5f, 1);
				} else if (me.race == "hobbit") {
					img.transform.localPosition = new Vector3(0.02f, -0.025f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.8f, 0.4f, 1);
				}
				
				continue;
			}


			// Gloves, Boots, Pants, Hair, Beard
			string path = "Tilesets/Basic/" + me.race + "-" + type;
			Sprite asset = Resources.Load<Sprite>(path);
			if (asset == null) {
				Debug.LogError(path + " not found");
			}

			img.sprite = asset;
			img.color = item.color;
		}
	}


	private Color GetRandomColor () {
		return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0, 1));
	}
}
