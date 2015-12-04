using UnityEngine;
using System.Collections;

public class Goblin : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		maxHp = 50;

		asset = Game.assets.monster["goblin"];
		base.Init(grid, x, y, scale, asset);
	}
}
