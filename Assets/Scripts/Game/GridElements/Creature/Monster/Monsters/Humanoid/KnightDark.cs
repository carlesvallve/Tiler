using UnityEngine;
using System.Collections;

public class KnightDark : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/KnightDark/knight-dark-" + Random.Range(1, 7));
		base.Init(grid, x, y, scale, asset);
	}
}
