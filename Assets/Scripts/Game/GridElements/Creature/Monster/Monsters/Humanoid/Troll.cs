using UnityEngine;
using System.Collections;

public class Troll : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Troll/troll-" + Random.Range(1, 3));
		base.Init(grid, x, y, scale, asset);

		stats.energyRate = 0.9f;
		stats.energy = Mathf.Max(1f, stats.energyRate);
	}
}
