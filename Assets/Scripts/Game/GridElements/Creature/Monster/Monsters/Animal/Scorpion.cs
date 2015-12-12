using UnityEngine;
using System.Collections;

public class Scorpion: Animal {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Animal/scorpion-" + Random.Range(1, 3);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		// stats
		isAgressive = true;
		SetEnergy(1.1f);
	}
}
