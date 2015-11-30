using UnityEngine;
using System.Collections;


public class Creature : Entity {

	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);

		SetImage(scale, new Vector3(0, 0.15f, 0), 0.04f);
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
	
}
