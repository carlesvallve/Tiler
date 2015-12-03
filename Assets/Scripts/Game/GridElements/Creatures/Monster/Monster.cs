using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Monster : Creature {

	Player player;

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);

		// Listen to game turn updates
		grid.player.OnGameTurnUpdate += () => {
			Think();
		};
	}


	protected virtual void Think () {
		// only monsters that see the player will think for now
		if (!IsVisible()) { return; }

		// move towards player
		MoveTowardsTarget(grid.player);
	}


	protected virtual void MoveTowardsTarget (Creature target) {
		// get increments toward the target
		Vector3 vec = (target.transform.position - transform.position).normalized;
		Point incs = new Point(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
		//print ("I see you! " + dx + "," + dy);

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
				if (grid.GetTile(x + 1, y + dy).IsWalkable()) {
					dx = 1;
				} else if (grid.GetTile(x - 1, y + dy).IsWalkable()) {
					dx = -1;
				}

			
			// left-right
			} else if (dy == 0 && dx != 0) {
				if (grid.GetTile(x + dx, y + 1).IsWalkable()) {
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
				print ("Where was I going?");
				Hud.instance.CreateLabel(transform.position, "?", Color.yellow);
				return new Point(0, 0); 
			}
		}

		return new Point(dx, dy);
	}

}
