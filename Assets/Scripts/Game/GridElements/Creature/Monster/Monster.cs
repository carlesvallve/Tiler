using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Monster : Creature {

	protected Tile targetTile;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset, id);

		// Each monster will evaluate what to do on each game turn update
		grid.player.OnGameTurnUpdate += () => {
			// escape if we are dying or already dead
			if (this == null) { return; }
			if (state == CreatureStates.Dying) { return; }
			if (grid.player.state == CreatureStates.Descending) { return; }

			// apply events that happen once per turn independent of energy rates
			RegenerateHp();

			// make monster take a decision
			Think();
		};
	}


	// =====================================================
	// Visibility
	// =====================================================

	public override void SetVisibility (Tile tile, bool visible, float shadowValue) {
		// when a monster becomes visible, player will have discovered this monster
		// this does no necessary mean that the monster sees the player yet
		if (!this.visible && visible) {
			// add to player's newVisibleMonsters
			grid.player.DiscoverMonster(this);
		} else if (this.visible && !visible) {
			// remove from player's newVisibleMonsters
			grid.player.UndiscoverMonster(this);
		}

		// update monster visibility
		base.SetVisibility(tile, visible, shadowValue);

		// set monster alert mode
		SetAlertMode(visible);

		if (!visible) {
			SetInfo("", Color.yellow);
		}
	}


	protected void SetAlertMode (bool visible) {
		// Monsters only see the player if he is inside the monster visionRadius
		float distanceToPlayer = Mathf.Round(Vector2.Distance(new Vector2(x, y), new Vector2(grid.player.x, grid.player.y)) * 10) / 10;
		
		if (distanceToPlayer < stats.vision) {
			// at this point, monster sees the player
			// if monster wasnt in alert mode, speak surprise message and start alert mode
			if (visible && stats.alert == 0) {
				Speak(isAgressive ? "Hey!" : "!", Color.white, 0, true); //, 0, true);
				stats.alert = stats.alertMax;
			}
		}
	}


	// =====================================================
	// Encounters
	// =====================================================

	protected override void ResolveCreatureEncounters (int x, int y) {
		Creature creature = grid.GetCreature(x, y);
		if (creature != null && creature != this) {
			if (creature is Player) {
				// set attack delay so all monsters dont attack at once
				float delay = grid.player.monsterQueue.Count * 0.25f;
				grid.player.monsterQueue.Add(this);

				// execute attack
				combatModule.Attack(creature, delay);
			}
		}
	}


	// =====================================================
	// Monster AI
	// =====================================================

	public override void Think () {
		// escape if we are doing something already
		/*if (state != CreatureStates.Idle) {
			return;
		}*/

		//if monster is not aware of the player, just freeze and do nothing
		/*if (!IsAware()) {
			return;
		}*/

		state = CreatureStates.Idle;

		// check if we have enough energy left to do something
		if (stats.energy <= 0) {
			stats.energy += stats.energyBase;
			return;
		} else {
			stats.energy -= 1;
		}
		
		// flee if we are afraid
		if (IsAfraid()) {
			bool success = Flee();
			if (!success ) { ChaseAndFollow(1); }

			MoveAgain();
			return;
		}


		// if we are not agresive, we are not interested on the player
		stats.interest[typeof(Player)] = IsAgressive() ? 100 : 0;

		// if we dont have a target tile, get a new one
		// if no interesting stuff was found, target will be a random tile to roam to
		if (this.targetTile == null) {
			this.targetTile = GetTarget();
		}

		// if we never got a valid target, do nothing this turn
		if (this.targetTile == null) {
			Debug.Log("Something went wrong with this monster decision. Will do nothing this turn...");
			return;
		}

		// chase target tile
		if (targetTile is Player) {
			if (IsAtShootRange(targetTile) && visible) {
				// shoot the player if in range
				combatModule.Shoot((Player)targetTile);
			} else {
				// use chase and follow algorithm
				ChaseAndFollow();
			}
			
		} else {
			// use astar pathfinding
			GotoTargetTile(this.targetTile); 
		}

		// move again if we have action points left
		MoveAgain();
	}
	

	protected void MoveAgain () {
		// tell game to wait until we finished our action, then think again
		float duration = state == CreatureStates.Moving ? speedMove : speed;
		Game.instance.StartCoroutine(Game.instance.WaitForTurnToEnd(this, duration));
	}


	protected void GotoTargetTile (Tile tile) {
		// search for new path to the target tile
		path = Astar.instance.SearchPath(this.x, this.y, tile.x, tile.y);

		// if no path, escape
		if (path.Count == 0) {
			//print ("Monser has no path. Escaping...");
			this.targetTile = null;
			return;
		}

		// cap path to next tile only
		path = new List<Vector2>() { path[0] };

		// follow the path
		StartCoroutine(FollowPath());

		// clear ai target if we arrived to it
		if ((int)path[0].x == tile.x && (int)path[0].y == tile.y) {
			this.targetTile = null;
		}
	}


	// =====================================================
	// Get AI Target
	// =====================================================

	protected Tile GetTarget () {
		Tile tile = null;

		// 1. choose a target in vision range
		tile = ChooseTargetByInterest(stats.vision);
		if (tile != null) {
			return tile;
		}

		// 2. if nothig interesting, choose a random tile to roam to
		tile = GetRandomTileInRadius(stats.vision);
		if (tile != null) {
			return tile;
		}

		// no available target was found
		return null;
	}


	protected Tile ChooseTargetByInterest (int radius) {
		// generate a list of all possible interesting tiles in radius, entities or creatures 
		// whose type is also defined in our interest dictionary
		List<Tile> list = new List<Tile>();

		// iterate on all tiles in radius
		for (int y = this.y + - radius; y <= this.y + radius; y++) {
			for (int x =  this.x - radius; x <= this.x + radius; x++) {
				if (x == this.x && y == this.y) {
					continue;
				}

				// check entities
				Entity entity = grid.GetEntity(x, y);
				if (entity != null) {
					System.Type type = entity.GetType();
					if (stats.interest.ContainsKey(type) && stats.interest[type] > 0) {
						list.Add(entity);
						entity.interestWeight = stats.interest[type];
					}
				}

				// check creatures
				Creature creature = grid.GetCreature(x, y);
				if (creature != null) {
					System.Type type = creature.GetType();
					if (stats.interest.ContainsKey(type) && stats.interest[type] > 0) {
						list.Add(creature);
						creature.interestWeight = stats.interest[type];
					}
				}
			}
		}

		// if we dont see anything interesting, escape
		if (list.Count == 0) {
			//print ("Nothing interesting in sight...");
			return null;
		}

		// now we have a list of items and creatures around with an interest weight that match out intereset dictionary
		// we want to pick a random tile from this list depending on the weight of the items in it
		Tile tile = Dice.GetRandomTileFromList(list);

		// now we have a target to act upon to
		return tile;
	}


	protected Tile GetRandomTileInRadius (int radius) {
		Tile tile = null;

		int c = 0;
		while (true) {
			int x = this.x + Random.Range(-radius, radius);
			int y = this.y + Random.Range(-radius, radius);

			if (x == this.x && y == this.y) {
				continue;
			}

			tile = grid.GetTile(x, y);
			if (tile && tile.IsWalkable()) {
				break;
			}

			c++;
			if (c == 100) {
				//print ("No target tile was found. Escaping..."); 
				return null; 
			}
		}

		return tile;
	}


	// =====================================================
	// ChaseAndFollow / Flee behaviours
	// =====================================================

	protected virtual bool Flee () {
		return ChaseAndFollow(-1);
	}


	protected virtual bool ChaseAndFollow (int direction = 1) { 
		// if we are a monster in attacking mood, set player as walkable so we can hit him
		grid.player.walkable = true; 

		// get best available tile for moving to while chasing/fleeing at/from the player
		// monsters can't follow LOS marks more than some number of (say 5?) rounds old.
		// (if we set it to 0, monster will only chase the player if they see him)
		Tile tile = GetTileWithBestFov(this.x, this.y, 5, direction);
		if (tile == null) { return false; }

		// return false in case we didnt find a better tile that the one we aer currently in
		// (used to turn to fight if we are fleeing)
		if (tile.x == this.x && tile.y == this.y) { 
			if (Vector2.Distance(new Vector2(grid.player.x, grid.player.y), new Vector2(this.x, this.y)) < 1.5f) {
				return false;
			}
		}

		// move to selected neighbour tile
		path = new List<Vector2>() { new Vector2(tile.x, tile.y) };
		if (path.Count == 0) { return false; }

		StartCoroutine(FollowPath());

		// restore player to unwalkable after this monster's action
		grid.player.walkable = false;

		return true;
	}


	private Tile GetTileWithBestFov (int x, int y, int maxTurnsOld, int order = 1) {
		// get neighbour tile with bigger fovTargetTurn and smallest fovDistance
		// neighbour must have some fov scent and be walkable
		List<Tile> neighbours = grid.GetPassableNeighbours(x, y, true);
		List<Tile> fovTiles = GetTilesWithBestFovTurn(neighbours, maxTurnsOld, order);
		Tile tile = GetTileWithBestFovDistance(fovTiles, order);

		return tile;
	}


	private List<Tile> GetTilesWithBestFovTurn (List<Tile> neighbours, int maxTurnsOld, int order = 1) { 
		// order -> 1: greatest value, -1: smallest value

		// generate an array with fov values
		int[] values = new int[neighbours.Count];
		for(int i = 0; i < neighbours.Count; i ++) {
			values[i] = neighbours[i].fovTurn;
		}

		// get the maximum value in the array
		int max = order == 1 ? Mathf.Max(values) : Mathf.Min(values);

		// generate a list with all tiles with maximum value
		List<Tile> tiles = new List<Tile>();
		foreach(Tile tile in neighbours) {

			if (order == 1) {
				// exclude tiles with no fov turn value
				if (tile.fovTurn == 0) { continue; }

				// exclude tiles with scent older than maxTurnsOld
				// if maxTurnsOld is 0, means that monster will only chase if they see the player
				if (order == 1 && tile.fovTurn < Game.instance.turn - maxTurnsOld) { continue; }
			}
			
			// exclude tiles with other monsters in it
			if (!tile.IsWalkable() && grid.GetCreature(tile.x, tile.y) != this) { continue; }

			if (tile.fovTurn == max) {
				tiles.Add(tile);
			}
		}

		return tiles;
	}


	private Tile GetTileWithBestFovDistance (List<Tile> tiles, int order = 1) {
		Tile selectedTile = null;

		float minDistance = order == 1 ? Mathf.Infinity : 0;
 
		for(int i = 0; i < tiles.Count; i ++) {
			Tile tile = tiles[i];

			if ((order == 1 && tile.fovDistance < minDistance) || (order == -1 && tile.fovDistance > minDistance)) {
				selectedTile = tile;
				minDistance = tile.fovDistance;
			}
		}

		return selectedTile;
	}


	// =====================================================
	// Old Ai Methods
	// =====================================================

	// This are old ai methods not used anymore, but who knows, we can re-apply it in the future
	// if pathfinding proves to be too expensive

	// =====================================================
	// MoveTowardsTarget
	// =====================================================

	/*protected virtual bool MoveTowardsTarget (Tile target) {
		// get increments toward the target
		Vector3 vec = (target.transform.position - transform.position).normalized;
		Point incs = new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
		
		// get increments after avoiding any obstacles
		incs = AvoidObstaclesInDirection(incs);



		// generate path with next position
		int x = this.x + incs.x;
		int y = this.y + incs.y;

		// if we still have an obstacle, return false to the function call
		if (!grid.GetTile(x, y).IsWalkable()) {
			return false;
		}

		path = new List<Vector2>() { new Vector2(x, y) };

		// move towards target
		StartCoroutine(FollowPath());

		return true;
	}

	
	// =====================================================
	// Avoid Obstacles In Direction
	// =====================================================

	private Point AvoidObstaclesInDirection (Point incs) {
		int c = 0;

		int dx = incs.x;
		int dy = incs.y;

		while (true) {
			// if tile in direction is not walkable
			Tile tile = grid.GetTile(x + dx, y + dy);
			if (tile.IsWalkable()) { break; }
			
			// up-down
			if (dx == 0 && dy != 0) {
				if (grid.GetTile(x + 1, y + dy).IsWalkable() && grid.GetTile(x - 1, y + dy).IsWalkable()) {
					int[] arr = new int[] { -1, 1 };
					dx = arr[Random.Range(0, arr.Length)];
				} else if (grid.GetTile(x + 1, y + dy).IsWalkable()) {
					dx = 1;
				} else if (grid.GetTile(x - 1, y + dy).IsWalkable()) {
					dx = -1;
				}

			
			// left-right
			} else if (dy == 0 && dx != 0) {
				if (grid.GetTile(x + dx, y + 1).IsWalkable() && grid.GetTile(x + dx, y - 1).IsWalkable()) {
					int[] arr = new int[] { -1, 1 };
					dy = arr[Random.Range(0, arr.Length)];
				} else if (grid.GetTile(x + dx, y + 1).IsWalkable()) {
					dy = 1;
				} else if (grid.GetTile(x + dx, y - 1).IsWalkable()) {
					dy = -1;
				}
			
			// diagonals
			} else if (dy != 0 && dx != 0) {
				int r = Random.Range(0, 2);
				if (r == 0) { dx = 0; }
				if (r == 1) { dy = 0; }
			}

			// escape if cant find a solution
			c++; 
			if (c == 100) {
				Speak("?", Color.white, true);
				return new Point(0, 0); 
			}
		}

		return new Point(dx, dy);
	}*/

}
