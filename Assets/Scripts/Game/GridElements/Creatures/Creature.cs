﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum CreatureStates  {
	Idle = 0,
	Moving = 1,
	Attacking = 2,
	Defending = 4,
	Using = 5
}


public class Creature : Tile {

	public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;

	public CreatureStates state { get; set; }

	public int hp = 5;

	protected List<Vector2> path;
	protected float speed = 0.15f;
	//public bool moving = false;
	//public bool stopAtNextTile = false;

	protected Creature target;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, new Vector3(0, 0.1f, 0), 0.04f);
		LocateAtCoords(x, y);

		state = CreatureStates.Idle;
	}


	protected virtual void LocateAtCoords (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);

		transform.localPosition = new Vector3(x, y, 0);

		SetSortingOrder(200);
	}


	/*protected IEnumerator WaitForGameTurnUpdate (float delay = 0) {
		if (this is Player) {
			if (delay > 0) {
				yield return new WaitForSeconds(delay);
			}
			if (OnGameTurnUpdate != null) { OnGameTurnUpdate.Invoke(); }
		}

		yield break;
	}*/


	// =====================================================
	// Path
	// =====================================================

	public virtual void SetPath (int x, int y) {
		// escape if creature is not in idle state
		if (state != CreatureStates.Idle) {
			return;
		}

		// clear previous path
		if (path != null) {
			DrawPath(Color.white);
		}

		// escape if goal is not visible
		Tile tile = grid.GetTile(x, y);
		if (tile == null) { return; }
		if (!tile.visible && !tile.explored) { return; }

		// if goal is the creature's tile, wait one turn instead
		if (x == this.x && y == this.y) {
			path = new List<Vector2>() { new Vector2(this.x, this.y) };
			Speak("...", Color.yellow);
		} else {
			// if we are the player goal is a creature, set goal tile as walkable
			if (this is Player) {
				Creature target = grid.GetCreature(x, y);
				if (target != null) {
					Astar.instance.walkability[target.x, target.y] = 0;
				}
			}
			

			// search for new path
			path = Astar.instance.SearchPath(grid.player.x, grid.player.y, x, y);
			path = SetPathAfterEncounter(path);
		}
		
		// escape if no path was found
		if (path.Count == 0) {
			StopMoving();
			return;
		}

		// render new path
		DrawPath(Color.magenta);

		// follow new path
		StartCoroutine(FollowPath());
	}


	protected void DrawPath (Color color) {
		if (path == null) { return; }

		foreach (Vector2 p in path) {
			Tile tile = grid.GetTile((int)p.x, (int)p.y);
			if (tile != null) {
				tile.SetColor(color);
			}
		}
	}


	// =====================================================
	// Movement
	// =====================================================

	protected IEnumerator FollowPath () {
		state = CreatureStates.Moving;

		for (int i = 0; i < path.Count; i++) {
			Hud.instance.Log("");

			// get next tile coords
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			yield return StartCoroutine (FollowPathStep(x, y));

			// escape if creature is not in moving state anymore
			if (state != CreatureStates.Moving) {
				yield break;
			}


			// emmit event
			if (this is Player) {
				if (OnGameTurnUpdate != null) { OnGameTurnUpdate.Invoke(); }
			}
		}

		// stop moving once we reach the goal
		StopMoving();

		// resolve encounters with current tile after moving
		ResolveEncountersAtGoal(this.x, this.y);
	}


	protected virtual void UpdatePosInGrid (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);
	}


	protected virtual IEnumerator FollowPathStep (int x, int y) {
		if (state != CreatureStates.Moving) { yield break; }

		// resolve encounters with next tile
		ResolveEntityEncounters(x, y);
		ResolveCreatureEncounters(x, y);

		// if we stopped moving because of encounters, wait and escape
		if (state != CreatureStates.Moving) { 
			//yield return new WaitForSeconds(1f);
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

			MoveToNextTile(x, y);

			yield return null;
		}

		// clear path color at tile
		grid.GetTile(x, y).SetColor(Color.white);

		if (this is Player) {
			sfx.Play("Audio/Sfx/Step/step", 0.8f, Random.Range(0.8f, 1.2f));
		}
	}


	protected void MoveToNextTile (int x, int y) {
		UpdateVision();
		UpdatePosInGrid(x, y);
	}


	public void StopMoving () {
		if (state == CreatureStates.Moving) {
			StopAllCoroutines();
		}

		state = CreatureStates.Idle;
		DrawPath(Color.white);
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


	private void ResolveEntityEncounters (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity == null) { return; }

		// resolve doors
		if (entity is Door) {
			Door door = (Door)entity;
			if (door.state != EntityStates.Open) {
				DrawPath(Color.white);

				// open the door
				if (door.state == EntityStates.Closed) { 
					
					state = CreatureStates.Using;
					StopMoving();
					CenterCamera();
					StartCoroutine(door.Open()); 
					if (this is Player) { 
						Hud.instance.Log("You open the door."); 
					}
				
				// unlock the door
				} else if (door.state == EntityStates.Locked) { 
					state = CreatureStates.Using;
					StopMoving();
					CenterCamera();
					StartCoroutine(door.Unlock(success => {
						if (this is Player) { 
							Hud.instance.Log(success ? "You unlock the door." : "The door is locked."); 
						}
					}));
				}
			}
		}
	}


	protected virtual void ResolveCreatureEncounters (int x, int y) {
		Creature creature = grid.GetCreature(x, y);
		if (creature != null && creature != this) {
			Attack(creature, 0);
		}
	}


	private void ResolveEncountersAtGoal (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// resolve stairs
			if ((this is Player) && (entity is Stair)) {
				//StopMoving();

				Stair stair = (Stair)entity;
				if (stair.state == EntityStates.Open) {
					StopMoving(); 
					state = CreatureStates.Using;
					Dungeon.instance.ExitLevel (stair.direction);
				} else {
					Hud.instance.Log("The stair doors are locked.");
				}
			}
		}
	}


	// =====================================================
	// Functions overriden by Player class
	// =====================================================

	protected virtual void MoveCameraTo (int x, int y) {}
	protected virtual void CenterCamera () {}
	public virtual void UpdateVision () {}


	// =====================================================
	// Attack
	// =====================================================

	protected void Attack (Creature target, float delay = 0) {
		if (state == CreatureStates.Attacking) { return; }
		
		StopMoving();
		state = CreatureStates.Attacking;

		//float delay = 0; //(this is Player) ? 0 : Random.Range(0.2f, 0.8f);
		StartCoroutine(AttackAnimation(target, delay));

		target.Defend(this, delay);
	}


	protected IEnumerator AttackAnimation (Creature target, float delay = 0) {
		yield return new WaitForSeconds(delay);
		
		float duration = speed * 0.5f;

		sfx.Play("Audio/Sfx/Combat/woosh", 0.6f, Random.Range(0.8f, 1.5f));

		// move towards target
		float t = 0;
		Vector3 startPos = transform.localPosition;
		Vector3 endPos = startPos + (target.transform.position - transform.position).normalized / 2;
		while (t <= 1) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		// move back to position
		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(endPos, startPos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		yield return null;
		state = CreatureStates.Idle;

		// emmit event
		if (this is Player) {
			if (OnGameTurnUpdate != null) { OnGameTurnUpdate.Invoke(); }
		}
	}


	// =====================================================
	// Defend
	// =====================================================

	protected void Defend (Creature attacker, float delay = 0) {
		if  (state == CreatureStates.Defending) { return; }

		StopMoving();

		state = CreatureStates.Defending;
		StartCoroutine(DefendAnimation(attacker, delay));
	}


	protected IEnumerator DefendAnimation (Creature attacker, float delay = 0) {
		yield return new WaitForSeconds(delay);

		// wait for impact
		float duration = speed * 0.5f;
		yield return new WaitForSeconds(duration);

		// resolve combat outcome
		int attack = Random.Range(1, 20);
		int defense = Random.Range(1, 20);

		// hit
		if (attack > defense) {
			int damage = Random.Range(1, 7);

			string[] arr = new string[] { "painA", "painB", "painC", "painD" };
			sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.1f, Random.Range(0.6f, 1.8f));
			sfx.Play("Audio/Sfx/Combat/hitB", 0.5f, Random.Range(0.8f, 1.2f));
			Speak("-" + damage, Color.red);

		// parry/dodge
		} else {
			int r = Random.Range(0, 2);
			if (r == 1) {
				string[] arr = new string[] { "swordB", "swordC" };
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.2f, Random.Range(0.6f, 1.8f));
				Speak("Parry", Color.white);
			} else {
				sfx.Play("Audio/Sfx/Combat/swishA", 0.1f, Random.Range(0.5f, 1.2f));
				Speak("Dodge", Color.white);
			}
		}

		// move towards attacker
		float t = 0;
		Vector3 startPos = new Vector3(this.x, this.y, 0);
		Vector3 vec = (new Vector3(attacker.x, attacker.y, 0) - startPos).normalized / 8;
		Vector3 endPos = startPos - vec; // * dir;
		while (t <= 1) {
			t += Time.deltaTime / duration * 0.5f;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		// move back to position
		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.5f);
			transform.localPosition = Vector3.Lerp(endPos, startPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		yield return null;
		state = CreatureStates.Idle;
	}
	
}

