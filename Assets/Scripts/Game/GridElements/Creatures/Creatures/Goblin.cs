using UnityEngine;
using System.Collections;

public class Goblin : Creature {

	public int hp = 0;

	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		
		hp = Random.Range(5, 10);
	}
}
