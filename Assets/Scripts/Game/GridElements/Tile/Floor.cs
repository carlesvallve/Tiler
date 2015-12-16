using UnityEngine;
using System.Collections;


public class Floor : Tile {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		//asset = Resources.Load<Sprite>("Tilesets/Dungeon/wall-gray-4"); //floor-gray");

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);
	}
	
}
