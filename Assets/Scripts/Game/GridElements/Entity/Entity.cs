using UnityEngine;
using System.Collections;

public enum EntityStates {
	None = 0,
	Open = 1,
	Closed = 2,
	Locked = 3
}


public class Entity: Tile {

	public EntityStates state { get; set; }

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetSortingOrder(100);
		SetImages(scale, new Vector3(0, 0.1f, 0), 0.04f);
	}


	public virtual void SetState (EntityStates state) {
		this.state = state;
	}


	protected override void UpdatePosInGrid (int x, int y) {
		grid.SetEntity(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetEntity(x, y, this);
	}
}
