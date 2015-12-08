﻿using UnityEngine;
using System.Collections;

public class Troll : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Troll/troll-" + Random.Range(1, 3));
		base.Init(grid, x, y, scale, asset);
	}
}