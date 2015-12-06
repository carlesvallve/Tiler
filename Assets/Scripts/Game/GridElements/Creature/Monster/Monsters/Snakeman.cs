using UnityEngine;
using System.Collections;

public class Snakeman : Monster {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Monster/Snakeman/snakeman-" + Random.Range(1, 8));
		base.Init(grid, x, y, scale, asset);
	}
}
