using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum CreatureStates  {
	Idle = 0,
	Moving = 1,
	Attacking = 2,
	Defending = 4,
	Dying = 5,
	Using = 6,
	Descending = 7
}


public class Creature : Tile {

	public delegate void GameTurnUpdateHandler();
	public event GameTurnUpdateHandler OnGameTurnUpdate;

	public delegate void GameOverHandler();
	public event GameOverHandler OnGameOver;

	protected List<Vector2> path;
	protected float speed = 0.15f;

	protected Creature target;

	public CreatureStates state { get; set; }

	public CreatureStats stats;
	public HpBar bar;
	//public int maxHp = 5;
	//public int hp = 5;

	
	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, new Vector3(0, 0.1f, 0), 0.04f);
		LocateAtCoords(x, y);

		state = CreatureStates.Idle;

		stats = new CreatureStats();
		InitStats();
		bar.Init(this);
	}


	protected virtual void InitStats () {	
	}


	protected virtual void UpdateHp (int ammount) {
		stats.hp += ammount; 

		if (stats.hp > stats.hpMax) { stats.hp = stats.hpMax; }
		if (stats.hp < 0) { stats.hp = 0; }

		bar.UpdateHp();
	}


	public virtual void LocateAtCoords (int x, int y) {
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
		// escape if creature is not in idle state
		if (state != CreatureStates.Idle) {
			return;
		}

		// clear previous path
		if (path != null) {
			DrawPath(Color.white);
		}

		// escape if goal has not benn explored yet
		Tile tile = grid.GetTile(x, y);
		if (tile == null) { return; }
		if (!tile.explored) { return; }

		// if goal is the creature's tile, wait one turn instead
		if (x == this.x && y == this.y) {
			path = new List<Vector2>() { new Vector2(this.x, this.y) };
			//Speak("...", Color.yellow);
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
			// get next tile coords
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			yield return StartCoroutine (FollowPathStep(x, y));
		}

		// stop moving once we reach the goal
		StopMoving();
	}


	protected virtual IEnumerator FollowPathStep (int x, int y) {
		if (state != CreatureStates.Moving) { 
			yield break; 
		}

		Hud.instance.Log("");
		

		// resolve encounters with next tile
		ResolveEntityEncounters(x, y);
		ResolveCreatureEncounters(x, y);



		// escape if we are no longer moving because of encounters on next tile
		if (state != CreatureStates.Moving) { 
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

			UpdatePosInGrid(x, y);

			yield return null;
		}

		// clear path color at tile
		grid.GetTile(x, y).SetColor(Color.white);

		// emit event
		if (this is Player) {
			sfx.Play("Audio/Sfx/Step/step", 0.8f, Random.Range(0.8f, 1.2f));
			if (OnGameTurnUpdate != null) { 
				OnGameTurnUpdate.Invoke(); 
			}
		}

		// resolve encounters with current tile after moving
		Vector2 goal = path[path.Count -1];
		if (this.x == (int)goal.x && this.y == (int)goal.y) {
			ResolveEncountersAtGoal(this.x, this.y);
		}
	}


	protected virtual void UpdatePosInGrid (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);
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
					CenterCamera();
					StartCoroutine(door.Open()); 
					if (this is Player) { 
						Hud.instance.Log("You open the door."); 
					}
				
				// unlock the door
				} else if (door.state == EntityStates.Locked) { 
					state = CreatureStates.Using;
					CenterCamera();
					StartCoroutine(door.Unlock(success => {
						if (this is Player) { 
							Hud.instance.Log(success ? "You unlock the door." : "The door is locked."); 
						}
					}));
				}
			}
		}

		// emmit event
		if (state == CreatureStates.Using) {
			if (this is Player) {
				if (OnGameTurnUpdate != null) { OnGameTurnUpdate.Invoke(); }
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
				Stair stair = (Stair)entity;

				if (stair.state == EntityStates.Open) {
					state = CreatureStates.Descending;
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
	public virtual void CenterCamera (bool interpolate = true) {}
	public virtual void UpdateVision (int x, int y) {}


	// =====================================================
	// Combat
	// =====================================================

	protected void Attack (Creature target, float delay = 0) {
		if (state == CreatureStates.Using) { return; }
		if (target.state == CreatureStates.Dying) { return; }
		
		StopMoving();
		state = CreatureStates.Attacking;

		StartCoroutine(AttackAnimation(target, delay));

		target.Defend(this, delay);
	}


	protected void Defend (Creature attacker, float delay = 0) {
		StopMoving();

		state = CreatureStates.Defending;
		StartCoroutine(DefendAnimation(attacker, delay));
	}

	protected void Die (Creature attacker, float delay = 0) {
		StopMoving();

		state = CreatureStates.Dying;
		StartCoroutine(DeathAnimation(attacker, delay));
	}


	private bool ResolveCombatOutcome (Creature attacker) {
		// resolve combat outcome
		int attack = attacker.stats.attack + Dice.Roll(1, 6);
		int defense = stats.defense + Dice.Roll(1, 6);

		// hit
		if (attack > defense) {
			int damage = attacker.stats.str + Dice.Roll(1, 4);
			if (damage > 0) {
				// apply damage
				UpdateHp(-damage);
				// display damage info
				string[] arr = new string[] { "painA", "painB", "painC", "painD" };
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.1f, Random.Range(0.6f, 1.8f));
				sfx.Play("Audio/Sfx/Combat/hitB", 0.5f, Random.Range(0.8f, 1.2f));
				Speak("-" + damage, Color.red);
				// create blood
				grid.CreateBlood(transform.position, damage);
				// set isDead to true
				if (stats.hp == 0) {
					return true;
				}
			}

		// parry or dodge
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

		return false;
	}

	// =====================================================
	// Combat Animations
	// =====================================================

	protected IEnumerator AttackAnimation (Creature target, float delay = 0) {
		yield return new WaitForSeconds(delay);
		if (target == null) { yield break; }
		
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

		state = CreatureStates.Idle;

		// emmit event
		if (this is Player) {
			if (OnGameTurnUpdate != null) { OnGameTurnUpdate.Invoke(); }
		}
	}


	protected IEnumerator DefendAnimation (Creature attacker, float delay = 0) {
		yield return new WaitForSeconds(delay);

		// wait for impact
		float duration = speed * 0.5f;
		yield return new WaitForSeconds(duration);

		// get combat positions
		Vector3 startPos = new Vector3(this.x, this.y, 0);
		Vector3 vec = (new Vector3(attacker.x, attacker.y, 0) - startPos).normalized / 8;
		Vector3 endPos = startPos - vec;

		// resolve combat outcome and apply combat sounds and effects

		bool isDead = ResolveCombatOutcome(attacker);
		if (isDead) {
			Die(attacker);
			yield break;
		}

		// move towards attacker
		float t = 0;
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

		state = CreatureStates.Idle;
	}


	protected IEnumerator DeathAnimation (Creature attacker, float delay = 0) {
		string[] arr = new string[] { "painA", "painB", "painC", "painD" };
		sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.3f, Random.Range(0.6f, 1.8f));
		sfx.Play("Audio/Sfx/Combat/hitB", 0.6f, Random.Range(0.5f, 2.0f));

		grid.CreateBlood(transform.localPosition, 16);

		// if player died, emit gameover event
		if (this is Player) {
			if (OnGameOver != null) { 
				OnGameOver.Invoke(); 
			}
		}

		Destroy(gameObject);
		yield break;
	}
}

