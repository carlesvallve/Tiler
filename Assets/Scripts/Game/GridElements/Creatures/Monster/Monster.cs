using UnityEngine;
using System.Collections;

public class Monster : Creature {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);

		// Listen to game turn updates
		grid.player.OnGameTurnUpdate += () => {
			//print ("Hey, the player is moving!");
		};

	}

}
