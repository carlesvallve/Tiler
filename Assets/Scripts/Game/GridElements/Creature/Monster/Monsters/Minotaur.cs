﻿using UnityEngine;
using System.Collections;

public class Minotaur : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Minotaur/minotaur-" + Random.Range(1, 11));
		base.Init(grid, x, y, scale, asset);
	}
}
