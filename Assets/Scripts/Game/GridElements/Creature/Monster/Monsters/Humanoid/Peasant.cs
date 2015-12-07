﻿using UnityEngine;
using System.Collections;

public class Peasant : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Peasant/peasant-" + Random.Range(1, 12));
		base.Init(grid, x, y, scale, asset);

		isAgressive = false;
	}
}
