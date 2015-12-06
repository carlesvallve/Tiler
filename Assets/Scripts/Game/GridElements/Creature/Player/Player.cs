using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Creature {

	protected int cameraMargin = 3;

	// list of monster that are currently attacking the player
	// used for calculating the monster attack delay, so they dont attack all at once
	public List<Monster> monsterQueue = new List<Monster>();


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = true;

		stats.hp = 20;
		stats.hpMax = 20;
		stats.attack = 3;
		stats.defense = 2;
		stats.str = 3;
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
		// clear monster queue
		monsterQueue.Clear();
		
		yield return StartCoroutine(base.FollowPathStep(x, y));

		// check if camera needs to track player
		CheckCamera();
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
						if (!creature.visible && tile.visible) {
							creature.Speak("Hey!", Color.white, true);
						}
						creature.SetVisibility(tile, lit[x, y], shadowValue);
					}
				}
			}
		}
	}
}
