using UnityEngine;
using System.Collections;


public class Gloves : Item {

	public int armour;

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Gloves/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Gloves";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"gauntlets-decorated", "gauntlets-iron", 
			"gloves-black", "gloves-decorated", "gloves-leather", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
	}

	private void SetStats (string assetName) {
		armour = 1;
		if (assetName == "gauntlets-iron") { armour = 2; }
	}

}
