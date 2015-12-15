using UnityEngine;
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

	// modules
	public CreatureMovement movement;
	public CreatureEncounters encounters;
	public CreatureCombat combat;
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

		// initialize creature modules
		movement = new CreatureMovement(this);
		encounters = new CreatureEncounters(this);
		combat = new CreatureCombat(this);
		inventory = new CreatureInventory(this);
	}


	/*void Update () {
		string energy = (Mathf.Round(stats.energy * 100f) / 100f).ToString();
		SetInfo(state.ToString() + "\n" + energy, Color.yellow);
	}*/


	// =====================================================
	// Stats
	// =====================================================


	public virtual void UpdateEquipmentStats () {
		stats.weapon = inventory.equipment["Weapon"] != null ? (Weapon)inventory.equipment["Weapon"].item : null;
		stats.shield = inventory.equipment["Shield"] != null ? (Shield)inventory.equipment["Shield"].item : null;

		stats.armour = stats.armourBase;
		if (inventory.equipment["Armour"] != null) { stats.armour += ((Armour)inventory.equipment["Armour"].item).armour; } 

		if (inventory.equipment["Hat"] != null) { stats.armour += ((Hat)inventory.equipment["Hat"].item).armour; } 
		if (inventory.equipment["Cloak"] != null) { stats.armour += ((Cloak)inventory.equipment["Cloak"].item).armour; } 
		if (inventory.equipment["Gloves"] != null) { stats.armour += ((Gloves)inventory.equipment["Gloves"].item).armour; } 
		if (inventory.equipment["Boots"] != null) { stats.armour += ((Boots)inventory.equipment["Boots"].item).armour; } 

		//stats.defense = stats.defenseBase;
		//if (inventory.equipment["Shield"] != null) { stats.defense += inventory.equipment["Shield"].defense; } 
 
		stats.attackRange = stats.weapon != null ? stats.weapon.range : 1;
	}


	public virtual void SetEnergy (float rate) {
		stats.energyRate = rate;
		stats.energy = Mathf.Max(1f, stats.energyRate);
	}


	public virtual bool UpdateEnergy () {
		//SetInfo(stats.energy.ToString(), Color.cyan);

		if (stats.energy < 1) {// stats.energyRate
			stats.energy += stats.energyRate;
			//stats.energy = Mathf.Round(stats.energy * 100f) / 100f;
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
		//if (path != null) { DrawPath(Color.white); }

		// escape if goal has not been explored yet
		Tile tile = grid.GetTile(x, y);
		if (tile == null) { return; }
		if (!tile.explored) { return; }

		// check the goal tile
		if (x == this.x && y == this.y) {
			// if goal is ourselves, wait one turn instead
			path = new List<Vector2>() { new Vector2(this.x, this.y) };
			if (this is Player) { 
				Hud.instance.Log("You wait..."); 
			}
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

		// stop moving once we reach the goal
		StopMoving();
	}


	protected virtual IEnumerator FollowPathStep (int x, int y) {
		// adjust speed to energy rate
		speedMove = Mathf.Min(0.15f / stats.energyRate, 0.15f);

		if (state != CreatureStates.Moving) { 
			yield break; 
		}

		// clear logs
		if (this is Player) { 
			Hud.instance.Log(""); 
		}
		
		// resolve encounters with next tile
		ResolveEntityEncounters(x, y);
		ResolveCreatureEncounters(x, y);

		// escape if we are no longer moving because of encounters on next tile
		if (state != CreatureStates.Moving) { 

			// wait enough time for monsters to complete their actions
			yield return new WaitForSeconds(speed); // is this enough always?

			// emit event if we used something
			if (state == CreatureStates.Using) {
				// emit event
				if (this is Player) {
					this.UpdateGameTurn();
					yield return null;
				}
			}

			// stop moving
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
		} else {
			if (this.visible) {
				sfx.Play("Audio/Sfx/Step/step", 0.4f, Random.Range(0.8f, 1.2f));
			}
			
		}

		// resolve encounters with current tile after moving
		bool escape = ResolveEncountersAtCurrentTile(this.x, this.y);
		if (escape) { yield break; }

		// emit event
		if (this is Player) {
			this.UpdateGameTurn();
			yield return null;
		}
	}


	protected override void UpdatePosInGrid (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);

		// update visibility
		UpdateVisibility();
	}


	public virtual void StopMoving () {
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
						combat.Shoot(target);
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
						combat.ShootToBreak(targetEntity);
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
			combat.Attack(creature, 0);
		}
	}


	protected bool ResolveEncountersAtCurrentTile (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// pickup items
			if (entity is Item) {
				((Item)entity).Pickup(this);
			}

			// check stairs at goal
			Vector2 goal = path[path.Count - 1];
			if (x == (int)goal.x && y == (int)goal.y) {
				if ((this is Player) && (entity is Stair)) {
					Stair stair = (Stair)entity;

					if (stair.state == EntityStates.Open) {
						state = CreatureStates.Descending;
						Dungeon.instance.ExitLevel (stair.direction);
						return true;
					} else {
						Hud.instance.Log("The stair doors are locked.");
					}
				}
			}
		}

		return false;
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

}

