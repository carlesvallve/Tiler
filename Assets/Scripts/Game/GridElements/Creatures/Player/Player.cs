using UnityEngine;
using System.Collections;

public class Player : Creature {

	public int hp = 10;


	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);	
	}

	public override void SetPath (int x, int y) {
		base.SetPath(x, y);

		// update game turns
		if (x == this.x && y == this.y) {
			Game.instance.UpdateTurn();
		}
	}


	protected override IEnumerator FollowPathStep (int i) {
		yield return StartCoroutine(base.FollowPathStep(i));
		Game.instance.UpdateTurn();
	}
}
