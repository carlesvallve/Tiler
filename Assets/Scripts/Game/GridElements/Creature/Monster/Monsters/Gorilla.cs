﻿using UnityEngine;
using System.Collections;

public class Gorilla : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Monkey/Gorilla/gorilla-" + Random.Range(1, 5));
		base.Init(grid, x, y, scale, asset);
	}
}
