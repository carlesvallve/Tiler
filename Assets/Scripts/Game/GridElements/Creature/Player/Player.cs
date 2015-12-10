using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Creature {

	public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;

	public delegate void GameOverHandler();
	public event GameOverHandler OnGameOver;

	protected bool cameraFollow = true;
	protected int cameraMargin = 3;

	protected string playerName;
	protected string playerRace;
	protected string playerClass;

	// list of monster that are currently attacking the player
	// used for calculating the monster attack delay, so they dont attack all at once
	public List<Monster> monsterQueue = new List<Monster>();


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		// set random name, race, class
		SetPlayerName();
		SetPlayerRace();
		SetPlayerClass();
		Hud.instance.LogPlayer(
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
		stats.hpMax = 16; 
		stats.hp = stats.hpMax;
		stats.visionRadius = 6;
		stats.attack = 5;
		stats.defense = 2;
		stats.str = 2;

		stats.attackRange = 5;
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

	// =====================================================
	// Event emission
	// =====================================================

	public override void UpdateGameTurn () {
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
		
		yield return StartCoroutine(base.FollowPathStep(x, y));

		// check if camera needs to track player
		CheckCamera();
	}


	// =====================================================
	// Encounters
	// =====================================================

	protected override void ResolveEntityEncounters (int x, int y) {
		base.ResolveEntityEncounters(x, y);

		// if stairs and is our last movement, disable camera follow
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && (entity is Stair)) {
			if (x == (int)(path[path.Count -1].x) && y == (int)(path[path.Count -1].y)) {
				cameraFollow = false;
			}
		}
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

		cameraFollow = true;
	}


	protected void CheckCamera () {
		if (!cameraFollow) {
			return;
		}

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
					float shadowValue = - 0.1f + Mathf.Min((distance / radius) * 0.6f, 0.6f);

					tile.SetVisibility(tile, lit[x, y], shadowValue);
					tile.SetFovInfo(Game.instance.turn, distance);

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
