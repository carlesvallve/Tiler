﻿using UnityEngine;
using System.Collections;

public class Demon : Humanoid {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Monster/Humanoid/Demon/demon-" + Random.Range(1, 4);
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		
		base.Init(grid, x, y, scale, asset);

		SetEnergy(1f);
	}
}
