using UnityEngine;
using System.Collections;

public class Player : Creature {

	protected bool useFovAlgorithm = true;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);	

		hp = 20;

		UpdateVision();
	}


	// =====================================================
	// Player messages
	// =====================================================

	protected void DisplayTileMessages (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity == null) { return; }

		// wait message
		if (x == this.x && y == this.y) {
			if ((entity is Stair)) { return; }
			Hud.instance.Log("You wait...");
		}

		// you see something message
		if (!entity.IsWalkable()) {
			if (entity.IsVisible()) {
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
		DisplayTileMessages(x, y);
	}


	protected override IEnumerator FollowPathStep (int i) {
		yield return StartCoroutine(base.FollowPathStep(i));
		
		// check if camera needs to track player
		CheckCamera();

		// update game turn
		Game.instance.UpdateTurn();
	}


	// =====================================================
	// Camera
	// =====================================================

	protected override void MoveCameraTo (int x, int y) {
		Camera2D.instance.StopAllCoroutines();
		Camera2D.instance.StartCoroutine(Camera2D.instance.MoveToPos(new Vector2(x, y)));
	}


	protected override void CenterCamera () {
		Camera2D.instance.StopAllCoroutines();
		Camera2D.instance.StartCoroutine(Camera2D.instance.MoveToPos(new Vector2(this.x, this.y)));
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

	protected override void UpdateVision () {
		if (!useFovAlgorithm) {
			return;
		}
		
		// TODO: We need to implement a Permissive Field of View algorithm instead, 
		// to avoid dark corners and get a better roguelike feeling

		// get lit array from shadowcaster class
		bool[,] lit = new bool[grid.width, grid.height];
		int radius = 6;

		ShadowCaster.ComputeFieldOfViewWithShadowCasting(
			this.x, this.y, radius,
			(x1, y1) => grid.TileIsOpaque(x1, y1),
			(x2, y2) => { lit[x2, y2] = true; });

		// iterate grid tiles and render them
		for (int y = 0; y < grid.height; y++) {
			for (int x = 0; x < grid.width; x++) {
				// render tiles
				Tile tile = grid.GetTile(x, y);
				if (tile != null) {
					float distance = Vector2.Distance(new Vector2(this.x, this.y), new Vector2(x, y));
					float shadowValue = - 0.1f + Mathf.Min((distance / radius) * 0.6f, 0.6f);

					tile.visible = lit[x, y];
					tile.gameObject.SetActive(lit[x, y] || tile.visited);
					tile.SetShadow(lit[x, y] ? shadowValue : 1);
					if (!lit[x, y] && tile.visited) { tile.SetShadow(0.6f); }

					// render entities
					Entity entity = grid.GetEntity(x, y);
					if (entity != null) {
						entity.visible = lit[x, y];
						entity.gameObject.SetActive(lit[x, y] || tile.visited);
						entity.SetShadow(lit[x, y] ? shadowValue : 1);
						if (!lit[x, y] && tile.visited) { entity.SetShadow(0.6f); }
					}

					// render creatures
					Creature creature = grid.GetCreature(x, y);
					if (creature != null) {
						creature.visible = lit[x, y];
						creature.gameObject.SetActive(lit[x, y] || tile.visited);
						creature.SetShadow(lit[x, y] ? shadowValue : 1);
						if (!lit[x, y] && tile.visited) { creature.SetShadow(0.6f); }
					}

					// mark lit tiles as visited
					if (lit[x, y]) { 
						tile.visited = true; 
					}
				}
			}
		}
	}

	
}
