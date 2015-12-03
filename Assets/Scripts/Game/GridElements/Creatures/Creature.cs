using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Creature : Entity {

	public int hp = 5;

	protected List<Vector2> path;
	protected float speed = 0.15f;
	protected bool moving = false;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
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


	// =====================================================
	// Path
	// =====================================================

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

		

		// escape if goal is not visible
		Tile tile = grid.GetTile(x, y);
		if (tile == null) { return; }
		if (!tile.gameObject.activeSelf) { return; }

		// if goal is the creature's tile, wait one turn instead
		if (x == this.x && y == this.y) {
			path = new List<Vector2>() { new Vector2(this.x, this.y) };
			Hud.instance.CreateLabel(transform.position, "...", Color.yellow);
		} else {
			// search for new path
			path = Astar.instance.SearchPath(grid.player.x, grid.player.y, x, y);
			path = SetPathAfterEncounter(path);
		}
		

		// escape if no path was found
		if (path.Count == 0) {
			//Hud.instance.Log("You cannot go there.");
			StopMoving();
			return;
		}

		// render new path
		DrawPath(Color.magenta);

		// follow new path
		StartCoroutine(FollowPath());
	}


	/*protected void Wait () {
		Hud.instance.Log("You wait...");
		Hud.instance.CreateLabel(transform.position, "...", Color.yellow);

		path = new List<Vector2>() { new Vector2(this.x, this.y) };
		StartCoroutine(FollowPath());
	}*/


	protected void DrawPath (Color color) {
		foreach (Vector2 p in path) {
			Tile tile = grid.GetTile((int)p.x, (int)p.y);
			if (tile != null) {
				tile.SetColor(color);
			}
		}
	}


	// =====================================================
	// Encounters
	// =====================================================

	private List<Vector2> SetPathAfterEncounter (List<Vector2> path) {
		int i;
		for (i = 0; i < path.Count; i ++) {
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			Entity entity = grid.GetEntity(x, y);

			// closed door
			if (entity is Door) {
				Door door = (Door)entity;
				if (door.state != EntityStates.Open) {
					break;
				}
			}
		}

		if (i < path.Count -1) {
			path.RemoveRange(i + 1, path.Count - 1 - i);
		}

		return path;
	}


	private IEnumerator ResolveEncounters (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// resolve doors
			if (entity is Door) {
				Door door = (Door)entity;
				if (door.state != EntityStates.Open) {
					DrawPath(Color.white);

					// closed door
					if (door.state == EntityStates.Closed) { 
						// open door
						moving = false;
						StartCoroutine(door.Open()); 
						Hud.instance.Log("You open the door.");
						CenterCamera();
			
					} else if (door.state == EntityStates.Locked) { 
						// unlock door
						moving = false;
						StartCoroutine(door.Unlock(success => {}));
						Hud.instance.Log("You unlock the door.");
						CenterCamera();
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
				//yield return new WaitForSeconds(0.25f);

				Stair stair = (Stair)entity;
				if (stair.state == EntityStates.Open) { 
					Dungeon.instance.ExitLevel (stair.direction);
				} else {
					Hud.instance.Log("The stair doors are locked.");
				}
			}
		}

		yield break;
	}


	// =====================================================
	// Movement
	// =====================================================

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

		// before moving, check for encounters on next tile in path
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

			MoveToNextTile();

			yield return null;
		}

		// update tile position in grid
		LocateAtCoords (x, y);
		sfx.Play("Audio/Sfx/Step/step", 0.8f, Random.Range(0.8f, 1.2f));

		// clear path color at tile
		grid.GetTile(x, y).SetColor(Color.white);
	}


	protected void MoveToNextTile () {
		if (!moving) { return; }

		Tile newTile = grid.GetTile(transform.localPosition);
		if (newTile.x == this.x && newTile.y == this.y) { return; }

		// update pos in grid
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(newTile.x, newTile.y, this);

		// pick items
		//PickupItemAtPos(transform.localPosition);

		// update vision
		UpdateVision();
	}


	private void StopMoving () {
		moving = false;
		DrawPath(Color.white);
		UpdateVision();
	}


	// =====================================================
	// Functions overriden by Player class
	// =====================================================

	protected virtual void MoveCameraTo (int x, int y) {}
	protected virtual void CenterCamera () {}
	protected virtual void UpdateVision () {}
	
}

