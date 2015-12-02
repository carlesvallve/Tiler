using UnityEngine;
using System.Collections;

public class Demon : Creature {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Game.assets.monster["demon"];
		base.Init(grid, x, y, scale, asset);
		
		hp = Random.Range(10, 20);
	}
}
