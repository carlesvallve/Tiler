using UnityEngine;
using System.Collections;

public class Player : Creature {


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);	

		hp = 20;

		UpdateVision();
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
