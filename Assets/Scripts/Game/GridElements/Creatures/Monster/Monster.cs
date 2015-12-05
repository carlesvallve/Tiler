using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
// CHASE&FOLLOW BEHAVIOUR IMPLEMENTED SUCCESSFULLY! :)

- for each lit tile in UpdateVision
	- store current turn
	- store current distance to player

- for each monster
	- determine if he wants to follow (always yes for now)
	- look at all neighbour tiles
	- choose the tile with biggest los
	- if all are equal, choose the want with shortest distance
	- move to that tile
*/


public class Monster : Creature {


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);

		// Each monster will evaluate what to do on each game turn update
		grid.player.OnGameTurnUpdate += () => {
			Think();
		};
	}


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
		// escape if we are dying or already dead
		if (this == null) { return; }
		if (state == CreatureStates.Dying) { return; }
		if (grid.player.state == CreatureStates.Descending) { return; }

		// move towards neighbour tile 
		//with best fov parameters
		ChaseAndFollow();
	}


	// =====================================================
	// Chase And Follow
	// =====================================================

	protected virtual void ChaseAndFollow () {
		// get best available tile for moving to while chasing the player
		Tile tile = GetTileWithBestFov(this.x, this.y);
		if (tile == null) { return; }

		// move to selected neighbour tile
		path = new List<Vector2>() { new Vector2(tile.x, tile.y) };
		StartCoroutine(FollowPath());
	}


	private List<Tile> GetNeighbours (int x, int y) {
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

		return neighbours;
	}


	private Tile GetTileWithBestFov (int x, int y) {
		// get neighbour tile with bigger fovTurn and smallest fovDistance
		// neighbour must have some fov scent and be walkable
		List<Tile> neighbours = GetNeighbours(x, y);
		List<Tile> fovTiles = GetTilesWithBestFovTurn(neighbours);
		Tile tile = GetTileWithBestFovDistance(fovTiles);

		return tile;
	}


	private List<Tile> GetTilesWithBestFovTurn (List<Tile> neighbours) {
		// generate an array with fov values
		int[] values = new int[neighbours.Count];
		for(int i = 0; i < neighbours.Count; i ++) {
			values[i] = neighbours[i].fovTurn;
		}

		// get the maximum value in the array
		int max = Mathf.Max(values);

		// generate a list with all tiles with maximum value
		List<Tile> tiles = new List<Tile>();
		foreach(Tile tile in neighbours) {

			// exclude tiles with no fov turn value
			if (tile.fovTurn == 0) { continue; }

			// exclude tiles with other monsters in it
			if (!tile.IsWalkable()) { continue; }

			if (tile.fovTurn == max) {
				tiles.Add(tile);
			}
		}

		return tiles;
	}


	private Tile GetTileWithBestFovDistance (List<Tile> tiles) {
		Tile selectedTile = null;

		float minDistance = Mathf.Infinity;
 
		for(int i = 0; i < tiles.Count; i ++) {
			Tile tile = tiles[i];

			if (tile.fovDistance < minDistance ) {
				selectedTile = tile;
				minDistance = tile.fovDistance;
			}
		}

		return selectedTile;
	}


	// =====================================================
	// MoveTowardsTarget + AvoidObstaclesInDirection
	// =====================================================

	/*protected virtual void MoveTowardsTarget (Creature target) {
		if (this == null) { return; }
		if (target == null) { return; }
		if (target.state == CreatureStates.Dying) { return; }

		// get increments toward the target
		Vector3 vec = (target.transform.position - transform.position).normalized;
		Point incs = new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
		
		//print ("I see you! " + incs.x + "," + incs.y);

		// get increments after avoiding any obstacles
		incs = AvoidObstaclesInDirection(incs);

		// generate path with next position
		int x = this.x + incs.x;
		int y = this.y + incs.y;
		path = new List<Vector2>() { new Vector2(x, y) };

		// move towards target
		StartCoroutine(FollowPath());
	}


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
	}*/

}
