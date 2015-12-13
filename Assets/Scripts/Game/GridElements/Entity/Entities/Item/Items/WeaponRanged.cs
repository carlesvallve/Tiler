using UnityEngine;
using System.Collections;


public class WeaponRanged : Item {

	public string damage = "1d4";
	public int range = 5;
	public int speed = 1;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Weapon/Ranged/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "WeaponRanged";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"bow-composite", "bow-long", "bow-short", 
			"crossbow-1", "crossbow-2", 
			"sling-1", "sling-2", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}
}
