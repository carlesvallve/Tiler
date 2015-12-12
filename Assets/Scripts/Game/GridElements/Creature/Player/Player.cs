﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Creature {

	public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;

	public delegate void GameOverHandler();
	public event GameOverHandler OnGameOver;

	protected int cameraMargin = 4;

	protected string playerName;
	protected string playerRace;
	protected string playerClass;

	// list of monster that are currently attacking the player
	// used for calculating the monster attack delay, so they dont attack all at once
	public List<Monster> monsterQueue = new List<Monster>();

	// list of monsters that enetered in view range this turn
	// used for displaying monster descriptions on encounters
	public List<Creature> newVisibleMonsters = new List<Creature>();


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
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

		// init stats
		stats.hpMax = 30; 
		stats.hp = stats.hpMax;
		stats.visionRadius = 6;
		stats.attack = 5;
		stats.defense = 2;
		stats.str = 2;
		stats.attackRange = 5;

		stats.energy = 1f;
		stats.energyRate = 1f;
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
		string[] classes = new string[] { "guard", "warrior", "ranger", "mage", "monk", "priest" }; // "normal", 
		playerClass = classes[Random.Range(0, classes.Length)];
	}


	protected void LogPlayerInfo () {
		string info = "";

		info = "LEVEL " + stats.level;
		info += "   XP: " + stats.xp + " / " + stats.xpMax;
		info += "   HP: " + stats.hp + " / " + stats.hpMax;
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
		Speak("!", Color.white, true);

		string str = "";
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
		//Utils.DebugList(newVisibleMonsters);

		// pick a random monster from the list and move camera to center pint between him and us
		Creature creature = newVisibleMonsters[Random.Range(0, newVisibleMonsters.Count)];
		Vector2 point = transform.localPosition + 
		(creature.transform.localPosition - transform.localPosition) / 2;
		MoveCameraTo((int)point.x, (int)point.y);
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
		int radius = stats.visionRadius;

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
