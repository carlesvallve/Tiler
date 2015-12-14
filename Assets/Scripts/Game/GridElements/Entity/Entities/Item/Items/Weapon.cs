using UnityEngine;
using System.Collections;


public class Weapon : Item {

	public string damage = "1d4";
	public int range = 1;
	public int speed = 1;

	// TODO: We need to implement at least one class per weapon type 
	// (Dagger, Sword, Axe, Mace, Staff, Bow, Crossbow, Sling...)


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string assetName = GetRandomAssetName();
		string path = "Tilesets/Item/Weapon/" + (IsRanged(assetName) ? "Ranged/" : "Melee/") + assetName;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }


		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Weapon";

		
		this.range = IsRanged(assetName) ? 5 : 1;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			// melee
			"axe-great", "axe-long", "axe-short", 
			"club", 
			"dagger", 
			"katana", 
			"mace", "mace-great", 
			"quarterstaff", 
			"sabre", "scimitar", 
			"sword-great", "sword-long", "sword-short", 

			// ranged
			"bow-composite", "bow-long", "bow-short", 
			"crossbow-1", "crossbow-2", 
			"sling-1", "sling-2", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}


	public bool IsRanged (string assetName) {
		string id = assetName.Split('-')[0];
		if (id =="bow" || id == "crossbow" || id == "sling") {
			return true;
		}

		return false;
	}
	
}
