using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Creature {

	public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;

	public delegate void GameOverHandler();
	public event GameOverHandler OnGameOver;

	protected int cameraMargin = 3;

	protected string playerName;
	protected string playerRace;
	protected string playerClass;

	// list of monster that are currently attacking the player
	// used for calculating the monster attack delay, so they dont attack all at once
	public List<Monster> monsterQueue = new List<Monster>();

	// list of monsters that enetered in view range this turn
	// used for displaying monster descriptions on encounters
	public List<Creature> newVisibleMonsters = new List<Creature>();


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		// set random name, race, class
		SetPlayerName();
		SetPlayerRace();
		SetPlayerClass();
		Hud.instance.LogPlayerName(
			Utils.UppercaseFirst(playerName) + ", the " +
			Utils.UppercaseFirst(playerRace) + " " +
			Utils.UppercaseFirst(playerClass)
		);

		// set asset
		//string path = "Tilesets/Monster/Humanoid/Hero/" + playerRace + "-" + playerClass;
		string path = "Tilesets/Basic/" + playerRace;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		// initialize
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		// set equipment tiles
		InitializeEquipment (scale);

		// initialize stats and equipment
		InitializeStats();
	}


	public void InitializeStats () {
		stats.type = "Humanoid";

		// init stats
		stats.hpMax = 20; 
		stats.hp = stats.hpMax;
		stats.vision = 6;
		stats.attack = 80;
		stats.defense = 60;
		stats.str = 2;

		// set initial items
		SetInitialItems();
	}


	// =====================================================
	// Equipment
	// =====================================================


	public override void SetInitialItems (int maxItems = 0, int minRarity = 100) {
		ItemGenerator generator = new ItemGenerator();

		if (playerClass == "guard") {
			// guard -> spear and buckler
			generator.GenerateSingle (this, typeof(Equipment), "ShortSpear");
			generator.GenerateSingle (this, typeof(Equipment), "Buckler");
		} else if (playerClass == "warrior") {
			// warrior -> sword and buckler
			generator.GenerateSingle (this, typeof(Equipment), playerRace == "dwarf" ? "ShortAxe" : "ShortSword");
			generator.GenerateSingle (this, typeof(Equipment), "Buckler");
		} else if (playerClass == "ranger") {
			// ranger -> bow
			generator.GenerateSingle (this, typeof(Equipment), playerRace == "dwarf" ? "LightCrossbow" : "ShortBow");
		} else if (playerClass == "mage" || playerClass == "priest") {
			// mage or priest -> book and staff
			generator.GenerateSingle (this, typeof(Book), null);
			generator.GenerateSingle (this, typeof(Equipment), "Quarterstaff");
		} else if (playerClass == "monk") {
			// monk -> book and potions
			generator.GenerateSingle (this, typeof(Book), null);
			for (int i = 1; i <=3; i++) {
				generator.GenerateSingle (this, typeof(Potion), null);
			}
		}

		generator.GenerateSingle (this, typeof(Equipment), "Cloak");

		// apply each generated item
		foreach(CreatureInventoryItem invItem in inventory.items) {
			ApplyItem(invItem);
		}
	}


	private void InitializeEquipment (float scale) {
		// Pants / boots / gloves
		GenerateEquipmentTile("Pants", "pants", scale, 0, Color.gray);
		GenerateEquipmentTile("Boots", "none", scale, 1, GetRandomColor());
		GenerateEquipmentTile("Gloves", "none", scale, 2, GetRandomColor());

		// hair
		string[] colors = new string[] { "#000000", "#ffff00", "#ff9900", "#ffffff", "#333333", "#A06400FF", "644600FF" };
		string hex = colors[Random.Range(0, colors.Length)];
		Color color = Color.red;
		ColorUtility.TryParseHtmlString (hex, out color);
		GenerateEquipmentTile("Hair", "hair", scale, 3, color);

		// beard
		string[] arr =new string[] { "none", "beard" };
		string beard = playerRace == "elf" ? "none" : arr[Random.Range(0, arr.Length)];
		GenerateEquipmentTile("Beard", beard, scale, 51, color);

		GenerateEquipmentTile("Armour", "none", scale, 50, Color.white);
		GenerateEquipmentTile("Hat", "none", scale, 100, Color.white);
		GenerateEquipmentTile("Weapon", "none", scale, 150, Color.white);
		GenerateEquipmentTile("Shield", "none", scale, 200, Color.white);
		GenerateEquipmentTile("Cloak", "none", scale, -100, Color.white);
	}


	private Color GetRandomColor () {
		return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0, 1));
	}


	private Tile GenerateEquipmentTile (string id, string type, float scale, int zIndexPlus, Color color) {
		Transform parent = transform;

		Sprite asset = null;
		if (type != "none") {
			string path = "Tilesets/Basic/" + playerRace + "-" + type;
			asset = Resources.Load<Sprite>(path);
			if (asset == null) {
				Debug.LogError(path + " not found");
			}
		}
		
		GameObject obj = (GameObject)Instantiate(grid.tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = id;

		Tile tile = obj.AddComponent<Tile>();
		tile.Init(grid, x, y, scale, asset, null);

		obj.transform.localPosition = Vector3.zero;

		tile.zIndex = zIndex + zIndexPlus;
		tile.SetSortingOrder();

		SpriteRenderer img = tile.transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>();
		img.color = color;
		img.transform.localPosition = new Vector3(-0.035f, 0.135f, 0);

		return tile;
	}


	protected override void RenderEquipment () {
		print ("Rendering equipment...");
		List<string> keys = new List<string>(inventory.equipment.Keys);
		List<CreatureInventoryItem> values = new List<CreatureInventoryItem>(inventory.equipment.Values);

		for (int i = 0; i < keys.Count; i++) {
			string key = keys[i];
			CreatureInventoryItem invItem = values[i];


			Transform tr = transform.Find(key);
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

				if (playerRace == "human" || playerRace == "elf") {
					img.transform.localPosition = new Vector3(0.015f, 0.01f, 0);
				} else if (playerRace == "dwarf") {
					img.transform.localPosition = new Vector3(0.015f, -0.075f, 0);
				} else if (playerRace == "hobbit") {
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

				if (playerRace == "human" || playerRace == "elf") {
					img.transform.localPosition = new Vector3(-0.075f, 0.225f, 0);
				} else if (playerRace == "dwarf") {
					img.transform.localPosition = new Vector3(-0.05f, 0.125f, 0);
				} else if (playerRace == "hobbit") {
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

				if (playerRace == "human" || playerRace == "elf") {
					img.transform.localPosition = new Vector3(0f, 0.175f, 0);
				} else if (playerRace == "dwarf") {
					img.transform.localPosition = new Vector3(0f, 0.05f, 0);
				} else if (playerRace == "hobbit") {
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

				if (playerRace == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.12f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.7f, 1);
				} else if (playerRace == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.12f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.7f, 1);
				} else if (playerRace == "dwarf") {
					img.transform.localPosition = new Vector3(-0.015f, 0.05f, 0);
					img.transform.localScale = new Vector3(1.2f, 0.6f, 1);
				} else if (playerRace == "hobbit") {
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

				if (playerRace == "human") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (playerRace == "elf") {
					img.transform.localPosition = new Vector3(0.01f, 0.125f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.9f, 0.55f, 1);
				} else if (playerRace == "dwarf") {
					img.transform.localPosition = new Vector3(-0.015f, 0.05f + 0.05f, 0);
					img.transform.localScale = new Vector3(1.0f, 0.5f, 1);
				} else if (playerRace == "hobbit") {
					img.transform.localPosition = new Vector3(0.02f, -0.025f + 0.05f, 0);
					img.transform.localScale = new Vector3(0.8f, 0.4f, 1);
				}
				
				continue;
			}


			// Gloves, Boots, Pants, Hair, Beard
			string path = "Tilesets/Basic/" + playerRace + "-" + type;
			Sprite asset = Resources.Load<Sprite>(path);
			if (asset == null) {
				Debug.LogError(path + " not found");
			}

			img.sprite = asset;
			img.color = item.color;
		}
	}


	protected void SetPlayerName () {
		if (Game.instance.gameNames != null) {
			playerName = Game.instance.gameNames["male"].GenerateRandomWord(Random.Range(3, 8));
		} else {
			Debug.Log("Game names have not been generated.");
		}
		
	}

	protected void SetPlayerRace () {
		string[] races = new string[] { "human", "dwarf", "elf", "hobbit" };
		playerRace = races[Random.Range(0, races.Length)];
	}


	protected void SetPlayerClass () {
		string[] classes = new string[] { "guard", "warrior", "ranger", "mage", "monk", "priest" };
		playerClass = classes[Random.Range(0, classes.Length)];
	}


	protected void LogPlayerInfo () {
		string info = "";

		//print (stats.xp);

		info = "LEVEL " + stats.level;
		info += "    XP: " + stats.xp + " / " + stats.xpMax;
		info += "    HP: " + stats.hp + " / " + stats.hpMax;

		info += "  " +
		"  <color='#ff0000'>STR " + stats.str + "</color>" +  
		"  <color='#00ff00'>DEX " + stats.dex + "</color>" +  
		"  <color='#00ffff'>CON " + stats.con + "</color>" +  
		"  <color='#ffff00'>INT " + stats.intel + "</color>";

		info += "    Combat: " + stats.attack + " / " + stats.defense;
		info += "    Gold: " + stats.gold;

		Hud.instance.LogPlayerInfo(info);
	}


	void Update() {
		LogPlayerInfo();
	}

	// =====================================================
	// Event emission
	// =====================================================

	public override void UpdateGameTurn () {
		// update player's vision
		UpdateVision(x, y);
		
		// if we discovered some new monsters, stop moving and log them
		LogNewVisibleMonsters();

		// emit update game turn event
		if (OnGameTurnUpdate != null) { 
			OnGameTurnUpdate.Invoke();
		}
	}


	public override void GameOver () {
		if (OnGameOver != null) { 
			OnGameOver.Invoke(); 
		}
	}


	// =====================================================
	// Path and Movement
	// =====================================================

	protected override IEnumerator FollowPathStep (int x, int y) {
		// clear monster queue
		monsterQueue.Clear();

		newVisibleMonsters.Clear();
		Hud.instance.Log("");
		
		yield return StartCoroutine(base.FollowPathStep(x, y));

		// check if camera needs to track player
		CheckCamera();

		// wait one frame more than other creatures
		//yield return null; //new WaitForSeconds(0.1f); //null;

		// if after all our actions, we discovered some new monsters, 
		// stop moving and log them
		LogNewVisibleMonsters();
	}


	// =====================================================
	// Encounters
	// =====================================================

	public virtual void DiscoverMonster (Creature creature) {
		if (state == CreatureStates.Descending) { return; }
		newVisibleMonsters.Add(creature);
	}


	public virtual void UndiscoverMonster (Creature creature) {
		foreach (Creature c in grid.player.newVisibleMonsters) {
			if (c == creature) {
				grid.player.newVisibleMonsters.Remove(creature);
				break;
			}
		}
	}


	private void LogNewVisibleMonsters () {
		if (state == CreatureStates.Descending) {
			return;
		}

		if (newVisibleMonsters.Count == 0) {
			return;
		}

		StopMoving();
		Speak("!", Color.white); //, true);

		/*string str = "";
		string punctuation = "";
		for (int i = 0; i < newVisibleMonsters.Count; i++) {
			if (newVisibleMonsters.Count > 1) {
				if (i > 0 && i < newVisibleMonsters.Count - 1) { punctuation = ", "; }
				if (i == newVisibleMonsters.Count - 1) { punctuation = " and "; }
			}
			string desc = newVisibleMonsters[i].GetType().ToString(); 
			str += punctuation + Utils.GetStringPrepositions(desc) + " " + desc; 
		}
		Hud.instance.Log("You see " + str);
		Utils.DebugList(newVisibleMonsters);*/

		// pick a random monster from the list and move camera to center pint between him and us
		Creature creature = newVisibleMonsters[Random.Range(0, newVisibleMonsters.Count)];
		Vector2 point = transform.localPosition + 
		(creature.transform.localPosition - transform.localPosition) / 2;
		MoveCameraTo((int)point.x, (int)point.y);

		Hud.instance.Log("You see " + 
			Descriptions.GetTileDescription(creature) + " " + 
			Descriptions.GetEquipmentDescription(creature)
		);
	}
	

	// =====================================================
	// Camera
	// =====================================================

	public override void MoveCameraTo (int x, int y) {
		Camera2D.instance.StopAllCoroutines();
		Camera2D.instance.StartCoroutine(Camera2D.instance.MoveToPos(new Vector2(x, y)));
	}


	public override void CenterCamera (bool interpolate = true) {
		if (state == CreatureStates.Descending) { 
			return; 
		}

		Camera2D.instance.StopAllCoroutines();

		if (interpolate) {
			Camera2D.instance.StartCoroutine(Camera2D.instance.MoveToPos(new Vector2(this.x, this.y)));
		} else {
			Camera2D.instance.LocateAtPos(new Vector2(this.x, this.y));
		}
	}


	protected void CheckCamera () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

		int margin = 16 + 32 * cameraMargin;
		if (screenPos.x < margin || screenPos.x > Screen.width - margin || 
			screenPos.y < margin || screenPos.y > Screen.height - margin) {
			CenterCamera();
		}
	}


	// =====================================================
	// Vision
	// =====================================================

	public override void UpdateVision (int px, int py) {
		// TODO: We need to implement a 'Permissive Field of View' algorithm instead, 
		// to avoid dark corners and get a better roguelike feeling

		// get lit array from shadowcaster class
		bool[,] lit = new bool[grid.width, grid.height];
		int radius = stats.vision;

		ShadowCaster.ComputeFieldOfViewWithShadowCasting(
			px, py, radius,
			(x1, y1) => grid.TileIsOpaque(x1, y1),
			(x2, y2) => { 
				if (grid.IsInsideBounds(x2, y2)) {
					lit[x2, y2] = true; 
				} else {
					//Debug.LogError ("ShadowCaster is out of bounds -> " + x2 + "," + y2);
				}
			});

		// iterate grid tiles and render them
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {
				// render tiles
				Tile tile = grid.GetTile(x, y);

				if (tile != null) {
					// render tiles (and record fov info)
					float distance = Mathf.Round(Vector2.Distance(new Vector2(px, py), new Vector2(x, y)) * 10) / 10;
					tile.SetFovInfo(Game.instance.turn, distance);

					// render tiles
					float shadowValue = - 0.1f + Mathf.Min((distance / radius) * 0.6f, 0.6f);
					tile.SetVisibility(tile, lit[x, y], shadowValue);

					// render entities
					Entity entity = grid.GetEntity(x, y);
					if (entity != null) { 
						entity.SetVisibility(tile, lit[x, y], shadowValue); 
					}

					// render creatures
					Creature creature = grid.GetCreature(x, y);
					if (creature != null) {
						creature.SetVisibility(tile, lit[x, y], shadowValue);
					}
				}
			}
		}
	}
}
