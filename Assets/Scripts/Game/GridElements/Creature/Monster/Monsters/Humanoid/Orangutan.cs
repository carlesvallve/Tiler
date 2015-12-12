using UnityEngine;
using System.Collections;

public class Orangutan : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Monkey/Orangutan/orangutan-" + Random.Range(1, 7));
		base.Init(grid, x, y, scale, asset);

		stats.energyRate = 0.75f;
	}
}
