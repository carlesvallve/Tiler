using UnityEngine;
using System.Collections;

public class Goblin : Creature {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Game.assets.monster["goblin"];
		base.Init(grid, x, y, scale, asset);
		
		hp = Random.Range(5, 10);
	}
}
