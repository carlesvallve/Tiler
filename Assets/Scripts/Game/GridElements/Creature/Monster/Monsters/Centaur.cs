﻿using UnityEngine;
using System.Collections;

public class Centaur : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Centaur/centaur-" + Random.Range(1, 11);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);
	}
}
