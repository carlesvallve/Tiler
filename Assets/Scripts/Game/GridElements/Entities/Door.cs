using UnityEngine;
using System.Collections;


public class Door : Entity {

	public EntityStates state { get; set; }


	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		walkable = true;

		SetImage(scale, Vector3.zero, 0.04f);

		state = EntityStates.Locked;
	}


	public void Open () {
		asset = Game.assets.dungeon["door-open"];
		outline.sprite = asset;
		img.sprite = asset;
		state = EntityStates.Open;
	}
	
}
