using UnityEngine;
using System.Collections;

public class Horse : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Animal/horse-" + Random.Range(1, 4);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		isAgressive = false;
	}
}
