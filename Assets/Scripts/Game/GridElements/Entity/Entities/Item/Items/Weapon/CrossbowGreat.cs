using UnityEngine;
using System.Collections;

public class CrossbowGreat : Weapon {

	
	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string assetName = GetRandomAssetName();
		string path = "Tilesets/Item/Weapon/Ranged/" + assetName;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		base.Init(grid, x, y, scale, asset);

		// stats
		this.damage = "2d8";
		this.speed = 1;
		this.range = 5;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { "crossbow-great" };
		return arr[Random.Range(0, arr.Length)];
	}

}

