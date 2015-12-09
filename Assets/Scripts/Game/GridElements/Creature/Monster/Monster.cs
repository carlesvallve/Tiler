using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: 

// - we need to implement basic ai thinking behaviours
//	- chase, flee, roam, chase item


public class MonsterAi {
	public Dictionary<System.Type, int> greed = new Dictionary<System.Type, int>() {
		{ typeof(Armour), 0 },
		{ typeof(Weapon), 0 },
		{ typeof(Book), 0 },
		{ typeof(Food), 0 },
		{ typeof(Potion), 0 },
		{ typeof(Treasure), 100 }
	};

	public Dictionary<System.Type, int> hate = new Dictionary<System.Type, int>() {
		{ typeof(Player), 0 },
		{ typeof(Monster), 0 },
	};

	public Dictionary<System.Type, int> fear = new Dictionary<System.Type, int>() {
		{ typeof(Player), 0 },
		{ typeof(Monster), 0 },
	};
}


public class Monster : Creature {

	

	public MonsterAi ai;
	protected Tile targetTile;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		
		InitializeAi();

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

	protected void InitializeAi () {
		// set each ai parameter here
		ai = new MonsterAi();
	}
	

	protected virtual void Think () {
		/*if (state != CreatureStates.Idle) {
			return;
		}*/

		// if has been surprised by the player, dont act this turn
		/*if (!IsAware()) {
			return;
		}*/

		// move towards/away from the player
		/*if (stats.alert > 0) {
			if (isAgressive) {
				ChaseAndFollow();
			} else {
				Flee();
			}
			return;
		}*/

		// if we dont have a target tile, get a new one
		if (this.targetTile == null) {
			this.targetTile = GetTarget();
		}

		// if we never got a valid target, do nothing this turn
		if (this.targetTile == null) {
			Debug.Log("Something went wrong with this monster decision...");
			return;
		}

		// goto target tile
		GotoTargetTile(this.targetTile);

		// chase item
		/*Item item = GetItemInRadius(stats.visionRadius);
		if (item != null) {
			GotoTarget(item);
		}*/

		// roam randomly
		//Roam();
	}


	protected Item GetBestItem () {
		Item item = null;
		// best item will be, from items in vision
		// list of item types with higher weight in ai greed dictionary
		// sort list by distance to the item
		// sort list by value of the item

		return item;
	}


	protected bool IsAware () {
		if (stats.alert > 0) { 
			if (!this.visible) { UpdateAlert(-1); }
			return true;
		}

		return false;
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
		//print ("Getting a new target: ");
		// 1. choose a random item in vision
		Item item = GetRandomItemInRadius(stats.visionRadius);
		if (item != null) {
			//print ("    going for " + item + " at " + x + "," + y);
			return item;
		}

		// 2. choose a random tile in vision
		Tile tile = GetRandomTileInRadius(stats.visionRadius);
		if (tile != null) {
			//print ("    roaming towards " + tile + " at " + x + "," + y);
			return tile;
		}

		// no available target was found
		return null;
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


	protected Item GetRandomItemInRadius (int radius) {
		List<Item> itemList = new List<Item>();

		for (int y = this.y + - radius; y <= this.y + radius; y++) {
			for (int x =  this.x - radius; x <= this.x + radius; x++) {
				if (x == this.x && y == this.y) {
					continue;
				}

				Entity entity = grid.GetEntity(x, y);
				if (entity != null && (entity is Item)) {
					Item item = (Item)entity;
					
					//print (item.GetType());
					item.weight = ai.greed[item.GetType()];
					//print (item + " " + item.weight);

					itemList.Add(item);
				}
			}
		}

		if (itemList.Count == 0) {
			//print ("No item was found. Escaping...");
			return null;
		}


		//Item selectedItem = Utils.RandomWeight(itemList);
		//return selectedItem;

		Utils.Shuffle(itemList);
		return itemList[0];
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


	private Tile GetTileWithBestFov (int x, int y, int maxTurnsOld, int order = 1) {
		// get neighbour tile with bigger fovTargetTurn and smallest fovDistance
		// neighbour must have some fov scent and be walkable
		List<Tile> neighbours = grid.GetPassableNeighbours(x, y, true);
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

	protected virtual bool MoveTowardsTarget (Tile target) {
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
	}


	// =====================================================
	// Random Roaming
	// =====================================================

	/*protected bool followingPath = false;

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
