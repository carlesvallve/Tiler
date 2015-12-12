using UnityEngine;
using System.Collections;

public class Giant : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Humanoid/Giant/giant-" + Random.Range(1, 3);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		stats.energyRate = 0.75f;
		stats.energy = Mathf.Max(1f, stats.energyRate);
	}
}
