using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetLoader;

// require necessary creature modules
[RequireComponent (typeof (CreatureInventory))]
[RequireComponent (typeof (CreatureCombat))]
[RequireComponent (typeof (CreatureEquipment))]


public class Creature : Tile {

	// basic
	public string race = "none";
	public string clase = "none";

	// movement
	protected List<Vector2> path;
	public float speed = 0.15f;
	public float speedMove = 0.15f;
	public bool markedToStop = false;

	// stats
	public CreatureStats stats;
	public HpBar bar;

	// states
	public CreatureStates state { get; set; }

	// ai
	protected Creature target;
	public bool isAgressive = true;

	// modules
	public CreatureCombat combatModule;
	public CreatureInventory inventoryModule;
	public CreatureEquipment equipmentModule;

	
	// =====================================================
	// Initialization
	// =====================================================

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		zIndex = 200;

		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, new Vector3(0, 0.1f, 0), 0.035f);

		state = CreatureStates.Idle;
		stats = new CreatureStats();
		bar.Init(this);

		LocateAtCoords(x, y);

		// initialize stats
		InitializeStats(id);
		if (stats.type == "Humanoid") {
			race = "human";
		}

		// initialize xp
		stats.xp = 0;
		stats.xpMax = 100 * stats.level;
		stats.xpValue = 20 * stats.level;

		// combat module (manages creature's combat logic and actions)
		combatModule = GetComponent<CreatureCombat>();
		combatModule.Init(this);

		// inventory module (manages inventory items and equipment)
		inventoryModule = GetComponent<CreatureInventory>();
		inventoryModule.Init(this);

		// equipment module (manages rendering equipment in creature's tile)
		equipmentModule = GetComponent<CreatureEquipment>();
		equipmentModule.Init(this);
	}


	public void InitializeStats (string id) {
		if (id == null) {
			return;
		}

		// assign props from csv
		MonsterData data = GameData.monsters[id];

		stats.id = data.id;
		stats.race = data.race;
		stats.type = data.type;
		stats.subtype = data.subtype;
		stats.rarity = data.rarity;

		stats.level = data.level;
		stats.hp = data.hp; stats.hpMax = data.hp;
		stats.energy = data.movement; stats.energyBase = data.movement;
		stats.attack = data.attack; stats.attackBase = data.attack;
		stats.defense = data.defense; stats.defenseBase = data.defense;
		stats.damage = data.damage; stats.damageBase = data.damage;
		stats.armour = data.armour; stats.armourBase = data.armour;
		stats.vision = data.vision;

		stats.xpValue = Mathf.RoundToInt((stats.hp + stats.armour) * stats.energy);

		// set asset
		Sprite[] assets = Assets.GetCategory("Monster/" + data.type + "/" + data.id);
		asset = assets[Random.Range(0, assets.Length)];
		SetAsset(asset);

		// set initial items (only for humanoids)
		// TODO: We should define in the csv what equipment each monster is able to wear
		/*if (stats.type == "Humanoid") {
			int minRarity = GameData.GetDefaultEquipmentMinRarity();
			SetInitialItems(Random.Range(0, 4), minRarity);
		} */
		
	}


	//void Update () {
		//string energy = (Mathf.Round(stats.energy * 100f) / 100f).ToString();
		//SetInfo(energy, Color.yellow);

		//SetInfo(stats.xpValue.ToString(), Color.yellow);

		//if (stats.weapon != null) { SetInfo((stats.weapon).id, Color.yellow); }

		//SetInfo(stats.attack + "/" + stats.defense, Color.yellow);
	//}

	
	// =====================================================
	// Stats
	// =====================================================

	
	// Experience

	public virtual void UpdateXp (int ammount) {
		stats.xp += ammount * 2; 

		// level up
		if (stats.xp >= stats.xpMax) { 
			LevelUp();
		}
	}


	protected void LevelUp () {
		// increase level
		stats.level += 1;
		stats.xp = stats.xp - stats.xpMax; 
		stats.xpMax = 100 * stats.level;
		stats.xpValue = 20 * stats.level;
		Speak("LEVEL UP", Color.green, 1.0f, true, 28);

		// increase hp
		int hpIncrease = Dice.Roll("1d6");
		stats.hpMax = stats.hpMax + hpIncrease;
		stats.hp = stats.hpMax;
		bar.UpdateHp();
		Speak ("HP +" + hpIncrease, Color.cyan, 1.5f, true, 28);

		// increase random stat
		UpdateRandomStat(1);

		// play levelup sound
		if (this is Player) {
			sfx.Play("Audio/Sfx/Stats/trumpets", 0.5f, Random.Range(0.8f, 1.2f));
		}
	}


	private void UpdateRandomStat (int ammount) {
		string[] statNames = new string[] { "STR", "DEX", "CON", "INT", "ATK", "DEF" };
		string statName = statNames[Random.Range(0, statNames.Length)];

		switch (statName) {
			case "STR": ammount *= 1; stats.str += ammount; break;
			case "DEX": ammount *= 1; stats.dex += ammount; break;
			case "CON": ammount *= 1; stats.con += ammount; break;
			case "INT": ammount *= 1; stats.intel += ammount; break;
			case "ATK": ammount *= Dice.Roll("2d4"); stats.attack += ammount; break;
			case "DEF": ammount *= Dice.Roll("2d4"); stats.defense += ammount; break;
		}

		Speak (statName + " +" + ammount, Color.cyan, 2.0f, true, 28);
	}


	// Health

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

		// iterate on all rendered equipment and set shadow too
		if (equipmentModule != null) {
			foreach (KeyValuePair<string, Tile> part in equipmentModule.parts) {
				if (part.Value == null) { continue; }

				part.Value.visible = visible; 
				part.Value.container.gameObject.SetActive(visible);

				part.Value.SetShadow(visible ? shadowValue : 1);
				if (!visible && tile.explored) { part.Value.SetShadow(0.6f); }
			};
		}
		


		// apply hpBar shadow
		bar.SetShadow(visible ? shadowValue : 1);
		if (!visible && tile.explored) { bar.SetShadow(0.6f); }

		// once we have seen the tile, mark the tile as explored
		if (visible) {
			tile.explored = true;
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
		if (stats.hp <= stats.hpMax * 0.25f) {
			return true;
		}

		return false;
	}


	protected int GetAttackRange () {
		return stats.weapon != null ? Mathf.Min(stats.weapon.range, stats.vision) : 1;
	}


	protected bool IsAtShootRange (Tile target) {
		float distance = Vector2.Distance(new Vector2(this.x, this.y), new Vector2(target.x, target.y));
		int range = GetAttackRange();

		if (range > 1 && distance <= range && distance > 1.44f) {
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

			if ((this is Player) && markedToStop) {
				break;
			}
		}

		// stop moving once we reach the goal
		StopMoving();
	}


	protected virtual IEnumerator FollowPathStep (int x, int y) {
		// adjust speed to energy rate
		speedMove = Mathf.Min(0.15f / stats.energyBase, 0.15f);

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
		//Tile tile = grid.GetTile(x, y);
		//ile.SetColor(tile.color);

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
						combatModule.Shoot(target);
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
						combatModule.ShootToBreak(targetEntity);
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
			combatModule.Attack(creature, 0);
		}
	}


	protected bool ResolveEncountersAtCurrentTile (int x, int y) {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {

			// pickup items
			if (entity is Item) {
				Pickup((Item)entity);
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
	// Inventory / Equipment
	// =====================================================

	public virtual void SetInitialItems (int maxItems = 0, int minRarity = 100) {
		ItemGenerator generator = new ItemGenerator();
		generator.Generate(this, maxItems, minRarity);

		// apply each generated item
		foreach(CreatureInventoryItem invItem in inventoryModule.items) {
			ApplyItem(invItem);
		}
	}


	public override System.Type GetRandomItemType () {
		// Pick a weighted random item type
		return Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
			{ typeof(Equipment), 	80 },
			{ typeof(Treasure), 	20 },
			{ typeof(Food), 		10 },
			{ typeof(Potion), 		5 },
			{ typeof(Book), 		3 },
		});
	}

	protected void Pickup (Item item) {
		// only humanoids can pickup items
		if (stats.type != "Humanoid") { return; }

		// tell the item to be picked up
		CreatureInventoryItem invItem = item.Pickup(this);

		// auto-use or auto-equip item if conditions are favourable
		ApplyItem(invItem);
	}


	protected void ApplyItem (CreatureInventoryItem invItem) {
		if (invItem.item.consumable) {
			// use consumable if we are low on hp
			if (stats.hp <= stats.hpMax * 0.5f) {
				inventoryModule.UseItem(invItem);
			}
			
		} else {
			// equip item if is better thatn what we own already
			if (inventoryModule.IsBestEquipment(invItem)) {
				inventoryModule.EquipItem(invItem);

				if (visible) {
					invItem.item.PlaySoundUse();
				}
			}
		}
	}


	public void UpdateEquipmentStats () {
		stats.weapon = inventoryModule.equipment["Weapon"] != null ? (Equipment)inventoryModule.equipment["Weapon"].item : null;
		stats.shield = inventoryModule.equipment["Shield"] != null ? (Equipment)inventoryModule.equipment["Shield"].item : null;
	}


	// =====================================================
	// Functions overriden by Player or Monster class
	// =====================================================

	public virtual void UpdateVision (int x, int y) {}

	public virtual void Think () {}
	protected virtual bool CanMove () { return true; }

	// event emission
	public virtual void UpdateGameTurn () {}
	public virtual void GameOver () {}

}

