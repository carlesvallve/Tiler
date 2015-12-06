using UnityEngine;
using System.Collections;


public class Wall : Entity {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Architecture/wall-gray");

		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetImages(scale, Vector3.zero, 0.04f);
	}
	
}
