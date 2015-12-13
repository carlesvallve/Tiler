using UnityEngine;
using System.Collections;


public class Weapon : Item {

	public string damage = "1d6";
	public int range = 1;
	public int speed = 1;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Weapon/Melee/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Weapon";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"axe-great", "axe-long", "axe-short", 
			"club", 
			"dagger", 
			"katana", 
			"mace", "mace-great", 
			"quarterstaff", 
			"sabre", "scimitar", 
			"sword-great", "sword-long", "sword-short", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}
	
}
