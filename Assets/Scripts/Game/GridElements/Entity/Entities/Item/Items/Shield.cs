using UnityEngine;
using System.Collections;


public class Shield : Item {

	public string defense = "1d4";


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		string path = "Tilesets/Item/Shield/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Shield";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"buckler-1", "buckler-2", "buckler-3", 
			"shield-elven", "shield-indestructible", "shield-kite", 
			"shield-large-1", "shield-large-2", "shield-large-3", 
			"shield-reflection", "shield-round", "shield-spriggan"
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}


	private void SetStats (string assetName) {
		defense = "1d4";
	}
	
}
