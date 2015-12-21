using UnityEngine;
using System.Collections;


public class Wall : Entity {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		walkable = false;

		//SetInfo(x + "," + y, Color.yellow);
	}
	
}
