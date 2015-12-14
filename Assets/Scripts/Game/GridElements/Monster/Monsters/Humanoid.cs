using UnityEngine;
using System.Collections;

public class Humanoid : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);

		isAgressive = true;

		SetEnergy(1f);
	}
}
