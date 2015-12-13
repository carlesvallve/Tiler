using UnityEngine;
using System.Collections;


public class Hat : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Hat/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Hat";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"cap-leather-1", "cap-leather-2", 
			"hat-feather", "hat-wizard-1", "hat-wizard-2", "hat-wizard-3", 
			"helmet-crested", "helmet-etched", "helmet-plumed-1", "helmet-plumed-2", "helmet-visored", "helmet-winged", 
			"mask-green",
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
	}

}
