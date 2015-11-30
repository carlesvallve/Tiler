using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Creature : Entity {

	protected List<Vector2> path;
	protected float speed = 0.15f;


	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		walkable = false;

		SetImage(scale, new Vector3(0, 0.1f, 0), 0.04f);
		LocateAtCoords(x, y);
	}


	protected virtual void LocateAtCoords (int x, int y) {
		grid.SetCreature(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetCreature(x, y, this);

		transform.localPosition = new Vector3(x, y, 0);

		SetSortingOrder(200);
	}


	public void SetPath (int x, int y) {
		// clear previous path
		if (path != null) {
			foreach (Vector2 p in path) {
				grid.GetTile((int)p.x, (int)p.y).SetColor(Color.white);
			}
		}

		// stop moving
		StopAllCoroutines();

		// search for new path
		path = Astar.instance.SearchPath(grid.player.x, grid.player.y, x, y);

		// render new path
		foreach (Vector2 p in path) {
			grid.GetTile((int)p.x, (int)p.y).SetColor(Color.magenta);
		}

		// floow new path
		StartCoroutine(FollowPath());
	}


	protected IEnumerator FollowPath () {
		for (int i = 0; i < path.Count; i++) {
			Vector2 p = path[i];
			int x = (int)p.x;
			int y = (int)p.y;

			LocateAtCoords (x, y);
			grid.GetTile(x, y).SetColor(Color.white);

			yield return new WaitForSeconds(speed);
		}
	}
	
}
