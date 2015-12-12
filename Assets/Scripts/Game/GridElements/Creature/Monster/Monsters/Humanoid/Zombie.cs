using UnityEngine;
using System.Collections;

public class Zombie : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Zombie/zombie-" + Random.Range(1, 11));
		base.Init(grid, x, y, scale, asset);

		SetEnergy(0.5f);
	}
}
