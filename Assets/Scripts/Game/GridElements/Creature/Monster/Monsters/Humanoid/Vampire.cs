using UnityEngine;
using System.Collections;

public class Vampire : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Vampire/vampire-" + Random.Range(1, 4));
		base.Init(grid, x, y, scale, asset);
	}
}
