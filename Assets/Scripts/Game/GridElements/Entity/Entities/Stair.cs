using UnityEngine;
using System.Collections;


public class Stair : Entity {

	public int direction = 1;
	

	public override void Init (Grid grid, int x, int y,float scale = 1, Sprite asset = null, string id = null) {
		asset = Resources.Load<Sprite>("Tilesets/Dungeon/Stair/stairs-down");

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		state = EntityStates.Open;
	}

	public override void SetState (EntityStates state) {
		this.state = state;
	}

	public void SetDirection (int direction) {
		this.direction = direction;

		if (direction == 1) {
			SetAsset(Resources.Load<Sprite>("Tilesets/Dungeon/Stair/stairs-down"));
		} else if (direction == -1) {
			SetAsset(Resources.Load<Sprite>("Tilesets/Dungeon/Stair/stairs-up"));
		}
	}

}
