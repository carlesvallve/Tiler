using UnityEngine;
using System.Collections;

public class Fly: Animal {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Animal/fly-" + Random.Range(1, 2);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		stats.energyRate = 0.5f;
	}
}
