using UnityEngine;
using System.Collections;

public class Dog: Animal {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Animal/dog-" + Random.Range(1, 5);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		// stats
		isAgressive = true;
		stats.energyRate = 2f;
	}
}
