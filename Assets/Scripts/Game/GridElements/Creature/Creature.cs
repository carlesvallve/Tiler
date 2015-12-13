﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// TODO; this class is beginning to be huge. We need to think a way of splitting it into modules:
// - movement
// - encounters
// - combat
// - etc...


public class Creature : Tile {

	// movement
	protected List<Vector2> path;
	public float speed = 0.15f;
	public float speedMove = 0.15f;

	// stats
	public CreatureStats stats;
	public HpBar bar;

	// states
	public CreatureStates state { get; set; }

	// ai
	protected Creature target;
	public bool isAgressive = true;

	// inventory
	public CreatureInventory inventory;

	
	// =====================================================
	// Initialization
	// =====================================================

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		zIndex = 200;

		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, new Vector3(0, 0.1f, 0), 0.035f);

		state = CreatureStates.Idle;
		stats = new CreatureStats();
		bar.Init(this);

		LocateAtCoords(x, y);

		// initialize energy
		SetEnergy(stats.energyRate);

		// initialize xp
		stats.xp = 0;
		stats.xpMax = 100 * stats.level;
		stats.xpValue = 20 * stats.level;

		// initialize inventory
		inventory = new CreatureInventory(this);
	}


	// =====================================================
	// Stats
	// =====================================================

	public virtual void SetEnergy (float rate) {
		stats.energyRate = rate;
		stats.energy = Mathf.Max(1f, stats.energyRate);
	}


	public virtual bool UpdateEnergy () {
		//SetInfo(stats.energy.ToString(), Color.cyan);

		if (stats.energy < 1) {
			stats.energy += stats.energyRate;
			stats.energy = Mathf.Round(stats.energy * 100f) / 100f;
			return false;
		}

		stats.energy -= 1;
		return true;
	}

	public virtual void UpdateXp (int ammount) {
		stats.xp += ammount; 

		// level up
		if (stats.xp >= stats.xpMax) { LevelUp(1); }
		if (stats.xp < 0) { LevelUp(-1); }
	}


	protected void LevelUp (int ammount) {
		stats.level += ammount;

		if (ammount > 0) { stats.xp = stats.xp - stats.xpMax; }
		if (ammount < 0) { stats.xp = stats.xpMax - stats.xp; } // check if this is correct (?)
		
		stats.xpMax = 100 * stats.level;
		stats.xpValue = 20 * stats.level;

		// increase hp
		int hpIncrease = Random.Range(1, 5);
		stats.hpMax = ammount > 0 ? stats.hpMax + hpIncrease : stats.hpMax - hpIncrease;
		if (stats.hp > stats.hpMax) { stats.hp = stats.hpMax; }
		bar.UpdateHp();

		// play levelup sound
		if (this is Player) {
			sfx.Play("Audio/Sfx/Stats/trumpets", 0.25f, Random.Range(0.8f, 1.2f));
		}
	}


	public virtual void UpdateHp (int ammount) {
		stats.hp += ammount; 

		if (stats.hp > stats.hpMax) { stats.hp = stats.hpMax; }
		if (stats.hp < 0) { stats.hp = 0; }

		bar.UpdateHp();
	}


	public virtual void RegenerateHp() {
		stats.regeneration += stats.regenerationRate;
		if (stats.regeneration >= 1) {
			UpdateHp(1);
			stats.regeneration = 0;
		}
	}


	public virtual void UpdateAlert (int ammount) {
		// only alerted monsters are able to chase the player
		// alert decreases each turn, when it reaches 0, 
		// monsters will be surprised by the player and wont move the turn they see him
		stats.alert += ammount; 

		if (stats.alert > stats.alertMax) { stats.alert = stats.alertMax; }
		if (stats.alert < 0) { stats.alert = 0; }
	}


	// =====================================================
	// Visibility
	// =====================================================

	public override void SetVisibility (Tile tile, bool visible, float shadowValue) {
		// creature is being seen by the player right now
		this.visible = visible; 
		container.gameObject.SetActive(visible);

		// apply shadow
		SetShadow(visible ? shadowValue : 1);
		if (!visible && tile.explored) { SetShadow(0.6f); }

		// apply hpBar shadow
		bar.SetShadow(visible ? shadowValue : 1);
		if (!visible && tile.explored) { bar.SetShadow(0.6f); }

		// once we have seen the tile, mark the tile as explored
		if (visible) {
			tile.explored = true;
		}
	}


	// =====================================================
	// Ai
	// =====================================================

	protected bool IsAware () {
		if (stats.alert > 0) { 
			if (!this.visible) { UpdateAlert(-1); }
			return true;
		}

		return false;
	}


	protected bool IsAgressive () {
		return isAgressive;
	}


	protected bool IsAfraid () {
		if (stats.hp < stats.hpMax * 0.5f) {
			return true;
		}

		return false;
	}


	protected bool IsRangedAttack () {
		return stats.attackRange > 1;
	}


	protected bool IsAtShootRange (Tile target) {
		float distance = Vector2.Distance(new Vector2(this.x, this.y), new Vector2(target.x, target.y));
		if (IsRangedAttack() && distance  >= 2 && distance <= stats.attackRange) {
			return true;
		}

		return false;
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
		/*if (path != null) {
			DrawPath(Color.white);
		}*/

		// escape if goal has not been explored yet
		Tile tile = grid.GetTile(x, y);
		if (tile == null) { return; }
		if (!tile.explored) { return; }

		// check the goal tile
		if (x == this.x && y == this.y) {
			// if goal is ourselves, wait one turn instead
			path = new List<Vector2>() { new Vector2(this.x, this.y) };
			if (this is Player) { Hud.instance.Log("You wait..."); }
			//Speak("...", Color.yellow);
		} else {

			// check for targets on the goal tile (for both chasing/melee and ranged attacks)
			bool escape = CheckForTargets(x, y);
			if (escape) {
				return;
			}
			
			// search for new path
			path = Astar.instance.SearchPath(this.x, this.y, x, y);
			path = CapPathToFirstEncounter(path);
		}
		
		// escape if no path was found
		if (path.Count == 0) {
			StopMoving();
			return;
		}

		// draw new path
		//DrawPath(Color.magenta);

		// follow new path
		StartCoroutine(FollowPath());
	}


	protected void DrawPath (Color color) {
		if (path == null) { return; }

		foreach (Vector2 p in path) {
			Tile tile = grid.GetTile((int)p.x, (int)p.y);
			if (tile != null) {
				tile.SetColor(color == Color.white ? tile.color : color);
			}
		}
	}


	// =====================================================
	// Movement
	// =====================================================

	protected virtual IEnumerator FollowPath () {
		state = CreatureStates.Moving;

		for (int i = 0; i < path.Count; i++) {
			// get next tile coords
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			yield return StartCoroutine (FollowPathStep(x, y));
		}

		// resolve encounters once we arrived to the goal
		ResolveEncountersAtGoal(this.x, this.y);

		// stop moving once we reach the goal
		StopMoving();
	}


	protected virtual IEnumerator FollowPathStep (int x, int y) {
		// adjust speed to energy rate
		speedMove = Mathf.Min(0.15f / stats.energyRate, 0.15f);

		if (state != CreatureStates.Moving) { 
			yield break; 
		}

		if (this is Player) { Hud.instance.Log(""); }
		
		// resolve encounters with next tile
		ResolveEntityEncounters(x, y);
		ResolveCreatureEncounters(x, y);

		// escape if we are no longer moving because of encounters on next tile
		if (state != CreatureStates.Moving) { 

			// wait enough time for monsters to complete their actions
			yield return new WaitForSeconds(speed);

			// emmit event if we used something
			if (state == CreatureStates.Using) {
				this.UpdateGameTurn();
			}
			
			// and stop moving
			StopMoving();
			yield break;
		}
		
		// interpolate creature position
		float t = 0;
		Vector3 startPos = transform.localPosition;
		Vector3 endPos = new Vector3(x, y, 0);
		while (t <= 1) {
			t += Time.deltaTime / speedMove;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

			UpdatePosInGrid(x, y);

			yield return null;
		}

		// clear path color at tile
		Tile tile = grid.GetTile(x, y);
		tile.SetColor(tile.color);

		// play step sound (player only)
		if (this is Player) {
			sfx.Play("Audio/Sfx/Step/step", 0.8f, Random.Range(0.8f, 1.2f));
		}

		// resolve encounters with current tile after moving
		ResolveEncountersAtCurrentTile(this.x, this.y);

		// emit event
		this.UpdateGameTurn();
	}


	protected override void UpdatePosInGrid (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);

		// update visibility
		UpdateVisibility();
	}


	protected virtual void StopMoving () {
		if (state == CreatureStates.Moving || state == CreatureStates.Using) {
			StopAllCoroutines();
		}

		state = CreatureStates.Idle;

		// clear path
		//DrawPath(Color.white);

		// creatures need to set their visibility also after they moved
		UpdateVisibility();
	}


	// =====================================================
	// Encounters
	// =====================================================

	protected bool CheckForTargets (int x, int y) {

		// if we are the player and goal is a creature, set goal tile as walkable
		if (this is Player) {
			Creature target = grid.GetCreature(x, y);
			if (target != null ) {
				Astar.instance.walkability[target.x, target.y] = 1;

				// if we have a ranged attack and we are in range, shoot the target
				if (target.visible) {
					if (IsAtShootRange(target)) {
						Shoot(target);
						return true;
					}
				}

				// otherwise, set target as walkable in astar walkability
				Astar.instance.walkability[target.x, target.y] = 0;
			}

			Entity targetEntity = grid.GetEntity(x, y);
			if (targetEntity != null && (targetEntity is Container) && targetEntity.state != EntityStates.Open) {
				Astar.instance.walkability[targetEntity.x, targetEntity.y] = 1;

				// if we have a ranged attack and we are in range, shoot the target
				if (targetEntity.breakable && targetEntity.visible) {
					if (IsAtShootRange(targetEntity)) {
						ShootToBreak(targetEntity);
						return true;
					}
				}
				
				// otherwise, set target as walkable in astar walkability
				Astar.instance.walkability[targetEntity.x, targetEntity.y] = 0;
			}
		}

		return false;
	}

	protected List<Vector2> CapPathToFirstEncounter (List<Vector2> path) {
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


	protected virtual void ResolveEntityEncounters (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity == null) { return; }

		// resolve closed/locked doors
		if (entity is Door) {
			Door door = (Door)entity;
			if (door.state != EntityStates.Open) {
				// open the door
				if (door.state == EntityStates.Closed) { 
					state = CreatureStates.Using;
					StartCoroutine(door.Open(this));
					return; 
					
				// unlock the door
				} else if (door.state == EntityStates.Locked) { 
					state = CreatureStates.Using;
					StartCoroutine(door.Unlock(this));
					return;
				}
			}
		}

		// resolve closed/locked containers
		if (entity is Container) {
			Container container = (Container)entity;
			if (container.state != EntityStates.Open) {
				// open the door
				if (container.state == EntityStates.Closed) { 
					state = CreatureStates.Using;
					StartCoroutine(container.Open(this));
					return; 
					
				// unlock the door
				} else if (container.state == EntityStates.Locked) { 
					state = CreatureStates.Using;
					StartCoroutine(container.Unlock(this));
					return;
				}
			}
		}
	}


	protected virtual void ResolveCreatureEncounters (int x, int y) {
		// if next tile is a creature, attack it
		Creature creature = grid.GetCreature(x, y);
		if (creature != null && creature != this) {
			Attack(creature, 0);
		}
	}


	protected void ResolveEncountersAtCurrentTile (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// pickup items
			if (entity is Item) {
				((Item)entity).Pickup(this);
			}
		}
	}

	protected void ResolveEncountersAtGoal (int x, int y) {
		if (path.Count == 0) { return; }

		Vector2 goal = path[path.Count - 1];
		if (x == (int)goal.x && y == (int)goal.y) {

			Entity entity = grid.GetEntity(x, y);
			if (entity != null) {

				// stairs (player only)
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
	}

	// =====================================================
	// Functions overriden by Player or Monster class
	// =====================================================

	public virtual void MoveCameraTo (int x, int y) {}
	public virtual void CenterCamera (bool interpolate = true) {}
	public virtual void UpdateVision (int x, int y) {}

	public virtual void Think () {}
	protected virtual bool CanMove () { return true; }

	// event emission
	public virtual void UpdateGameTurn () {}
	public virtual void GameOver () {}


	// =====================================================
	// Combat
	// =====================================================

	protected void Shoot (Creature target, float delay = 0) {
		StopMoving();

		Attack(target, delay);

		// create bullet
		grid.CreateBullet(transform.localPosition, target.transform.localPosition, speed, 8, Color.yellow);
	}

	public void ShootToBreak (Entity target) {
		StopMoving();

		// create bullet
		grid.CreateBullet(transform.localPosition, target.transform.localPosition, speed, 8, Color.yellow);

		// break container
		Container container = (Container)target;
		container.StartCoroutine(container.Open(this));
	}


	public void AttackToBreak (Entity target) {
		float delay = state == CreatureStates.Moving ? speed : 0;
		StartCoroutine(AttackAnimation(target, delay, 4));
	}
	

	protected void Attack (Creature target, float delay = 0) {
		if (state == CreatureStates.Using) { return; }
		if (target.state == CreatureStates.Dying) { return; }

		//stats.energy = stats.energyRate;

		state = CreatureStates.Attacking;

		StartCoroutine(AttackAnimation(target, delay, 3));

		target.Defend(this, delay);
	}


	protected void Defend (Creature attacker, float delay = 0) {
		StopMoving();

		state = CreatureStates.Defending;
		StartCoroutine(DefendAnimation(attacker, delay, 8));
	}

	
	private bool ResolveCombatOutcome (Creature attacker) {
		// resolve combat outcome
		int attack = attacker.stats.attack + Dice.Roll("1d8+2");
		int defense = stats.defense + Dice.Roll("1d6+1");

		// hit
		if (attack > defense) {
			int damage = attacker.stats.str + Dice.Roll("1d4");
			if (damage > 0) {
				// apply damage
				UpdateHp(-damage);

				// display damage info
				string[] arr = new string[] { "painA", "painB", "painC", "painD" };
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.1f, Random.Range(0.6f, 1.8f));
				sfx.Play("Audio/Sfx/Combat/hitB", 0.5f, Random.Range(0.8f, 1.2f));
				Speak("-" + damage, Color.red);

				// create blood
				grid.CreateBlood(transform.position, damage, Color.red);
				
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
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.15f, Random.Range(0.6f, 1.8f));
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

	protected IEnumerator AttackAnimation (Tile target, float delay = 0, float advanceDiv = 2) {
		yield return new WaitForSeconds(delay);
		if (target == null) { yield break; }
		
		float duration = speed * 0.5f;

		sfx.Play("Audio/Sfx/Combat/woosh", 0.4f, Random.Range(0.5f, 1.5f));

		// move towards target
		float t = 0;
		Vector3 startPos = transform.localPosition;
		Vector3 endPos = startPos + (target.transform.position - transform.position).normalized / advanceDiv;
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
	}


	protected IEnumerator DefendAnimation (Creature attacker, float delay = 0, float advanceDiv = 8) {
		yield return new WaitForSeconds(delay);

		// wait for impact
		float duration = speed * 0.5f;
		yield return new WaitForSeconds(duration);

		// get combat positions
		Vector3 startPos = new Vector3(this.x, this.y, 0);
		Vector3 vec = (new Vector3(attacker.x, attacker.y, 0) - startPos).normalized / advanceDiv;
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

		// emmit event once the defender has finished this action
		attacker.UpdateGameTurn();
	}


	// =====================================================
	// Death
	// =====================================================

	protected void Die (Creature attacker, float delay = 0) {
		StopMoving();

		state = CreatureStates.Dying;
		StartCoroutine(DeathAnimation(attacker, delay));

		// get a list of all items carried by the creature
		List<Item> allItems = new List<Item>();
		foreach (CreatureInventoryItem invItem in inventory.items) {
			allItems.Add(invItem.item);
		}

		/*List<Item> allItems = new List<Item>();
		foreach (List<Item> itemCategory in inventory.Values) {
			foreach(Item item in itemCategory) {
				allItems.Add(item);
			}
		}*/

		// spawn all the items carried by the creature
		SpawnItemsFromInventory(allItems);
	}


	protected IEnumerator DeathAnimation (Creature attacker, float delay = 0) {
		string[] arr = new string[] { "painA", "painB", "painC", "painD" };
		sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.3f, Random.Range(0.6f, 1.8f));
		sfx.Play("Audio/Sfx/Combat/hitB", 0.6f, Random.Range(0.5f, 2.0f));

		grid.CreateBlood(transform.localPosition, 16, Color.red);

		grid.SetCreature(this.x, this.y, null);
		Destroy(gameObject);

		// update attacker xp
		attacker.UpdateXp(stats.xpValue);

		// if player died, emit gameover event
		if (this is Player) {
			this.GameOver();
		}
		
		

		yield break;
	}

}

