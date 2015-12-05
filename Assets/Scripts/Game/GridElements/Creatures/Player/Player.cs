using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Creature {

	/*public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;*/

	protected bool useFovAlgorithm = true;

	// list of monster that are currently attacking the player
	// used for calculating the monster attack delay, so they dont attack all at once
	public List<Monster> monsterQueue = new List<Monster>();


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = true;

		stats.hp = 20;
		stats.hpMax = 20;
		stats.attack = 2;
		stats.defense = 2;
		stats.str = 2;
	}


	// =====================================================
	// Player messages
	// =====================================================

	protected void DisplayFooterMessages (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity == null) { return; }

		// wait message
		if (x == this.x && y == this.y) {
			if ((entity is Stair)) { return; }
			Hud.instance.Log("You wait...");
		}

		// you see something message
		if (!entity.IsWalkable()) {
			if (entity.visible) {
				string[] arr = entity.asset.name.Split('-'); 
				Hud.instance.Log("You see a " + arr[0]);
				MoveCameraTo(x, y);
			} else {
				Hud.instance.Log("Your eyes stair into the darkness...");
			}
		}
	}
	

	// =====================================================
	// Path and Movement
	// =====================================================

	public override void SetPath (int x, int y) {
		base.SetPath(x, y);
		DisplayFooterMessages(x, y);
	}


	protected override IEnumerator FollowPathStep (int x, int y) {

		monsterQueue.Clear();
		
		yield return StartCoroutine(base.FollowPathStep(x, y));

		// check if camera needs to track player
		CheckCamera();

		/*// emit update game turn event
		OnGameTurnUpdate.Invoke();*/
	}


	// =====================================================
	// Camera
	// =====================================================

	protected override void MoveCameraTo (int x, int y) {
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

		int margin = 16 + 32 * 3;
		if (screenPos.x < margin || screenPos.x > Screen.width - margin || 
			screenPos.y < margin || screenPos.y > Screen.height - margin) {
			CenterCamera();
		}

		/*if (screenPos.x < Screen.width * 0.25f || screenPos.y < Screen.height * 0.25f || 
			screenPos.x > Screen.width * 0.75f || screenPos.y > Screen.height * 0.75f) {
			CenterCamera();
		}*/
	}


	// =====================================================
	// Vision
	// =====================================================

	public override void UpdateVision (int px, int py) {
		if (!useFovAlgorithm) {
			return;
		}

		//return;
		
		// TODO: We need to implement a Permissive Field of View algorithm instead, 
		// to avoid dark corners and get a better roguelike feeling

		// get lit array from shadowcaster class
		bool[,] lit = new bool[grid.width, grid.height];
		int radius = 6;

		ShadowCaster.ComputeFieldOfViewWithShadowCasting(
			px, py, radius,
			(x1, y1) => grid.TileIsOpaque(x1, y1),
			(x2, y2) => { lit[x2, y2] = true; });

		// iterate grid tiles and render them
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {
				// render tiles
				Tile tile = grid.GetTile(x, y);
				if (tile != null) {
					float distance = Vector2.Distance(new Vector2(px, py), new Vector2(x, y));
					float shadowValue = - 0.1f + Mathf.Min((distance / radius) * 0.6f, 0.6f);

					tile.visible = lit[x, y];
					tile.SetVisible(lit[x, y] || tile.explored);
					tile.SetShadow(lit[x, y] ? shadowValue : 1);
					if (!lit[x, y] && tile.explored) { tile.SetShadow(0.6f); }

					// render entities
					Entity entity = grid.GetEntity(x, y);
					if (entity != null) {

						entity.visible = lit[x, y];
						entity.SetVisible(lit[x, y] || tile.explored);
						entity.SetShadow(lit[x, y] ? shadowValue : 1);
						if (!lit[x, y] && tile.explored) { entity.SetShadow(0.6f); }
					}

					// render creatures
					Creature creature = grid.GetCreature(x, y);
					if (creature != null) {

						creature.visible = lit[x, y];
						creature.SetVisible(lit[x, y] || tile.explored);
						creature.SetShadow(lit[x, y] ? shadowValue : 1);
						if (!lit[x, y] && tile.explored) { creature.SetShadow(0.6f); }

						// render hp bar
						creature.bar.SetShadow(lit[x, y] ? shadowValue : 1);
						if (!lit[x, y] && tile.explored) { creature.bar.SetShadow(0.6f); }
					}

					// mark lit tiles as explored
					if (lit[x, y]) { 
						tile.explored = true; 

						// generate tile fov info, used by monster ai
						if (tile.IsPassable()) {
							// record fovTile and fovDistance in this tile
							tile.fovTurn = Game.instance.turn;
							float fovDistance = Vector3.Distance(new Vector2(tile.x, tile.y), new Vector2(px, py));
							tile.fovDistance = Mathf.Round(fovDistance * 10) / 10;

							// debug fov info
							string debug = tile.fovTurn.ToString() + "\n" + tile.fovDistance.ToString();
							tile.SetInfo(debug, Color.white);
						} 
					} else {
						// decrease tile fov info until it gets to 0
						// (tiles lose their scent over time)
						/*if (tile.IsPassable()) {
							tile.fovTurn -= 1;
							if (tile.fovTurn < 0) { tile.fovTurn = 0; }
							string debug = tile.fovTurn.ToString() + "\n" + tile.fovDistance.ToString();
							tile.SetInfo(debug, Color.white);
						}*/
					}
				}
			}
		}
	}
}
