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

		SetImage(scale, new Vector3(0, 0.1f, 0), 0.04f);
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


	public void SetPath (int x, int y) {
		// clear previous path
		if (path != null) {
			DrawPath(Color.white);
		}

		// if already moving, abort current move
		if (moving) {
			moving = false;
			return;
		}

		// search for new path
		path = Astar.instance.SearchPath(grid.player.x, grid.player.y, x, y);

		// render new path
		DrawPath(Color.magenta);

		// floow new path
		StartCoroutine(FollowPath());
	}


	protected void DrawPath (Color color) {
		foreach (Vector2 p in path) {
			grid.GetTile((int)p.x, (int)p.y).SetColor(color);
		}
	}


	protected IEnumerator FollowPath () {
		moving = true;

		for (int i = 0; i < path.Count; i++) {
			// get next tile coords
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			// before moving, we want to check for encounters on next coord in path
			yield return StartCoroutine(ResolveEncounters(x, y));

			// escape if we stopped moving for any reason
			if (!moving) { 
				DrawPath(Color.white);
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
		}

		moving = false;
	}


	private IEnumerator ResolveEncounters (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// resolve doors
			if (entity is Door) {
				Door door = (Door)entity;
				if (door.state == EntityStates.Closed) { // open door
					yield return StartCoroutine(door.Open());
				} else if (door.state == EntityStates.Locked) { // locked door
					yield return StartCoroutine(door.Unlock(success => {
						moving = success; // stop moving if we could not unlock it
					}));
				}
			}
		}

		yield break;
	}
	
}








