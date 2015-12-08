using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: 

// - we need to implement basic ai thinking behaviours
//	- chase, flee, roam, chase item


public class Monster : Creature {


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);

		debugEnabled = true;

		// Each monster will evaluate what to do on each game turn update
		grid.player.OnGameTurnUpdate += () => {
			// escape if we are dying or already dead
			if (this == null) { return; }
			if (state == CreatureStates.Dying) { return; }
			if (grid.player.state == CreatureStates.Descending) { return; }

			// set monster actions
			RegenerateHp();
			Think();
		};
	}


	// =====================================================
	// Visibility
	// =====================================================

	public override void SetVisibility (Tile tile, bool visible, float shadowValue) {
		base.SetVisibility(tile, visible, shadowValue);

		// manage monster alert mode
		SetAlertMode(visible);
	}


	protected void SetAlertMode (bool visible) {
		// Monsters only see the player if he is inside the monster visionRadius
		float distanceToPlayer = Mathf.Round(Vector2.Distance(new Vector2(x, y), new Vector2(grid.player.x, grid.player.y)) * 10) / 10;
		
		if (distanceToPlayer < stats.visionRadius) {
			// if monster wasnt in alert mode, speak surprise message and start alert mode
			if (visible && stats.alert == 0) {
				Speak(isAgressive ? "Hey!" : "!", Color.yellow, true);
				stats.alert = stats.alertMax;
			}
		}

		//SetInfo(distanceToPlayer.ToString(), Color.yellow);
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
				Attack(creature, delay);
			}
		}
	}


	// =====================================================
	// Monster AI
	// =====================================================

	protected virtual void Think () {

		// if has been surprised by the player, dont act this turn
		if (!IsAware()) {
			return;
		}

		// move towards/away from the player
		if (stats.alert > 0) {
			if (isAgressive) {
				ChaseAndFollow();
			} else {
				Flee();
			}
			return;
		}

		// roam randomly
		//Roam();
	}


	protected bool IsAware () {
		if (stats.alert > 0) { 
			if (!this.visible) { UpdateAlert(-1); }
			return true;
		}

		return false;
	}


	// =====================================================
	// ChaseAndFollow / Flee behaviours
	// =====================================================

	protected virtual void Flee () {
		ChaseAndFollow(-1);
	}


	protected virtual void ChaseAndFollow (int direction = 1) { 
		// get best available tile for moving to while chasing/fleeing at/from the player
		// monsters can't follow LOS marks more than some number of (say 5?) rounds old.
		// (if we set it to 0, monster will only chase the player if they see him)
		Tile tile = GetTileWithBestFov(this.x, this.y, 5, direction); // x, y, maxTurnsOld, direction(chase or flee)
		if (tile == null) { return; }

		// move to selected neighbour tile
		path = new List<Vector2>() { new Vector2(tile.x, tile.y) };
		StartCoroutine(FollowPath());
	}


	private List<Tile> GetNeighbours (int x, int y, bool addCenterTile = false) {
		Tile[] tiles = new Tile[] {
			grid.GetTile(x + 0, y - 1), 
			grid.GetTile(x + 1, y - 1),
			grid.GetTile(x + 1, y + 0),
			grid.GetTile(x + 1, y + 1),
			grid.GetTile(x + 0, y + 1),
			grid.GetTile(x - 1, y + 1),
			grid.GetTile(x - 1, y + 0),
			grid.GetTile(x - 1, y - 1)
		};

		List<Tile> neighbours = new List<Tile>();
		foreach (Tile tile in tiles) {
			if (tile != null && tile.IsPassable()) {
				neighbours.Add(tile);
			}
		}

		if (addCenterTile) {
			neighbours.Add(grid.GetTile(x, y));
		}

		return neighbours;
	}


	private Tile GetTileWithBestFov (int x, int y, int maxTurnsOld, int order = 1) {
		// get neighbour tile with bigger fovTargetTurn and smallest fovDistance
		// neighbour must have some fov scent and be walkable
		List<Tile> neighbours = GetNeighbours(x, y, true);
		List<Tile> fovTiles = GetTilesWithBestFovTurn(neighbours, maxTurnsOld, order);
		Tile tile = GetTileWithBestFovDistance(fovTiles, order);

		return tile;
	}


	private List<Tile> GetTilesWithBestFovTurn (List<Tile> neighbours, int maxTurnsOld, int order = 1) { // 1: greatest, -1: smallest
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


	private Tile GetTileWithBestFovDistance (List<Tile> tiles, int order = 1) { // 1: smallest, -1: greatest
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
	// MoveTowardsTarget
	// =====================================================

	/*protected virtual void MoveTowardsTarget (Tile target) {
		// get increments toward the target
		Vector3 vec = (target.transform.position - transform.position).normalized;
		Point incs = new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
		
		// get increments after avoiding any obstacles
		incs = AvoidObstaclesInDirection(incs);

		// generate path with next position
		int x = this.x + incs.x;
		int y = this.y + incs.y;
		path = new List<Vector2>() { new Vector2(x, y) };

		// move towards target
		StartCoroutine(FollowPath());
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
				Speak("?", Color.white);
				return new Point(0, 0); 
			}
		}

		return new Point(dx, dy);
	}


	// =====================================================
	// Random Roaming
	// =====================================================

	protected bool followingPath = false;

	protected virtual void Roam () {
		// move in a random direction until we are too far away from original point
		// if we are, tend to return to it
		if (state == CreatureStates.Attacking || state == CreatureStates.Defending) {
			path = null;
			return;
		}

		// escape if we are following a previous path
		if (path != null && path.Count > 1) {

			if (state != CreatureStates.Moving) { 
				path = null;
				return;
			}
			StartCoroutine(FollowPathStep((int)path[0].x, (int)path[0].y));
			if (path.Count > 1) { path.RemoveAt(0); }
			return;
		}

		int radius = 8;
		Tile tile = null;
		while (true) {
			int xx = x + Random.Range(-radius, radius);
			int yy = x + Random.Range(-radius, radius);
			tile = grid.GetTile(xx, yy);
			if (tile && tile.IsWalkable()) {
				break;
			}
		}	

		SetPath(tile.x, tile.y);
	}


	public override void SetPath (int x, int y) {
		// search for new path
		path = Astar.instance.SearchPath(this.x, this.y, x, y);
		path = CapPathToFirstEncounter(path);

		if (path != null && path.Count > 0) {
			state = CreatureStates.Moving;
			StartCoroutine(FollowPathStep((int)path[0].x, (int)path[0].y));
			if (path.Count > 1) { path.RemoveAt(0); }
		}
	}


	// =====================================================
	// Random Roaming
	// =====================================================

	protected virtual void MoveToItem () {
		// update vision without render

		// get most interesting item

		// set path to item

		// follow path
	}*/

}
