using UnityEngine;
using System.Collections;

public class Monkey : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Humanoid/Monkey/Monkey/monkey-" + Random.Range(1, 7));
		base.Init(grid, x, y, scale, asset);
	}
}
