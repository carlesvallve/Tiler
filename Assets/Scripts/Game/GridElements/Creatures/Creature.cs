using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Creature : Entity {

	protected List<Vector2> path;
	protected float speed = 0.15f;
	protected bool moving = false;


	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		walkable = false;

		SetImages(scale, new Vector3(0, 0.1f, 0), 0.04f);
		LocateAtCoords(x, y);
	}


	protected virtual void LocateAtCoords (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);

		transform.localPosition = new Vector3(x, y, 0);

		SetSortingOrder(200);
	}


	public virtual void SetPath (int x, int y) {

		// clear previous path
		if (path != null) {
			DrawPath(Color.white);
		}

		// if already moving, abort current move
		if (moving) {
			StopMoving();
			return;
		}

		// if goal is the creature's tile, wait one turn instead
		if (x == this.x && y == this.y) {
			Hud.instance.Log("You wait...");
			return;
		}

		// search for new path
		path = Astar.instance.SearchPath(grid.player.x, grid.player.y, x, y);

		// render new path
		DrawPath(Color.magenta);

		// follow new path
		StartCoroutine(FollowPath());
	}


	protected void DrawPath (Color color) {
		foreach (Vector2 p in path) {
			Tile tile = grid.GetTile((int)p.x, (int)p.y);
			if (tile != null) {
				tile.SetColor(color);
			}
		}
	}


	protected IEnumerator FollowPath () {
		moving = true;

		for (int i = 0; i < path.Count; i++) {
			yield return StartCoroutine (FollowPathStep(i));
		}

		// after moving, check for encounters on goal tile
		yield return StartCoroutine(ResolveEncountersAtGoal(this.x, this.y));

		StopMoving();
	}


	protected virtual IEnumerator FollowPathStep (int i) {
		Hud.instance.Log("");

		// get next tile coords
		Vector2 p = path[i];
		int x = (int)p.x;
		int y = (int)p.y;

		// before moving, we want to check for encounters on next tile in path
		yield return StartCoroutine(ResolveEncounters(x, y));

		// escape if we stopped moving for any reason
		if (!moving) { 
			StopMoving();
			yield break;
		}

		// interpolate creature position
		float t = 0;
		Vector3 startPos = transform.localPosition;
		Vector3 endPos = new Vector3(x, y, 0);
		while (t <= 1) {
			t += Time.deltaTime / speed;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		// update tile position in grid
		LocateAtCoords (x, y);
		sfx.Play("Audio/Sfx/Step/step", 0.8f, Random.Range(0.8f, 1.2f));

		// clear path color at tile
		grid.GetTile(x, y).SetColor(Color.white);

		// check if camera needs to track player
		CheckCamera();
	}


	private void CheckCamera () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

		if (screenPos.x < Screen.width * 0.25f || screenPos.y < Screen.height * 0.25f || 
			screenPos.x > Screen.width * 0.75f || screenPos.y > Screen.height * 0.75f) {

			Camera2D.instance.StopAllCoroutines();
			Camera2D.instance.StartCoroutine(Camera2D.instance.MoveToPos(new Vector2(this.x, this.y)));
		}
	}


	private void StopMoving () {
		moving = false;
		DrawPath(Color.white);
	}


	private IEnumerator ResolveEncounters (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// resolve doors
			if (entity is Door) {
				Door door = (Door)entity;
				if (door.state != EntityStates.Open) {
					DrawPath(Color.white);

					if (door.state == EntityStates.Closed) { // open door
						moving = false;
						StartCoroutine(door.Open()); // yield return
						Hud.instance.Log("You open the door.");
			
					} else if (door.state == EntityStates.Locked) { // locked door
						moving = false;
						StartCoroutine(door.Unlock(success => {})); // yield return 
						Hud.instance.Log("You unlock the door.");
					}
				}
			}
		}

		yield break;
	}


	private IEnumerator ResolveEncountersAtGoal (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// resolve stairs
			if (entity is Stair) {
				StopMoving();

				yield return new WaitForSeconds(0.25f);

				Stair stair = (Stair)entity;
				Dungeon.instance.ExitLevel (stair.direction);

			}
		}

		yield break;
	}
	
}








