using UnityEngine;
using System.Collections;

public class Zombie : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Zombie/zombie-" + Random.Range(1, 11));
		base.Init(grid, x, y, scale, asset);

		stats.energyRate = 0.5f;
		stats.energy = Mathf.Max(1f, stats.energyRate);
	}
}
