using UnityEngine;
using System.Collections;


public class Door : Entity {

	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		walkable = true;

		SetImage(scale, Vector3.zero, 0.04f);
	}
	
}
