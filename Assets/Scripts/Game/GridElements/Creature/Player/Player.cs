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
		string path = "Tilesets/Monster/Humanoid/Hero/" + playerRace + "-" + playerClass;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		// initialize
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		InitializeStats();
	}


	public void InitializeStats () {
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

		// apply each generated item
		foreach(CreatureInventoryItem invItem in inventory.items) {
			ApplyItem(invItem);
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
		info += "   XP: " + stats.xp + " / " + stats.xpMax;
		info += "   HP: " + stats.hp + " / " + stats.hpMax;

		info += "  " +
		" <color='#ff0000'>STR " + stats.str + "</color>" +  
		" <color='#00ff00'>DEX " + stats.dex + "</color>" +  
		" <color='#00ffff'>CON " + stats.con + "</color>" +  
		" <color='#ffff00'>INT " + stats.intel + "</color>";

		info += "   Combat: " + stats.attack + " / " + stats.defense;
		info += "   Gold: " + stats.gold;

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
