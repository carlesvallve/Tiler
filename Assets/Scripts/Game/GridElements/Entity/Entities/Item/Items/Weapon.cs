using UnityEngine;
using System.Collections;


public class Weapon : Item {

	//public string attack = "1d6";
	//public string damage = "1d4";
	//public int range = 1;
	//public int speed = 1;

	public string id;
	public string type;
	public string adjective;

	public int hit;
	public string damage;
	public int range;
	public int hands;
	public int weight;


	// TODO: We need to implement at least one class per weapon type 
	// (Dagger, Sword, Axe, Mace, Staff, Bow, Crossbow, Sling...)


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		/*string assetName = GetRandomAssetName();
		string path = "Tilesets/Item/Weapon/" + (IsRanged(assetName) ? "Ranged/" : "Melee/") + assetName;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }*/


		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Weapon";

		//this.range = IsRanged(assetName) ? 5 : 1;
		InitializeStats(id);
	}


	public void InitializeStats (string id) {
		// assign props from csv
		WeaponData data = GameData.weapons[id];

		id = data.id;
		type = data.type;
		adjective = data.adjective;

		hit = data.hit;
		damage = data.damage;
		range = data.range;
		hands = data.hands;
		weight = data.weight;

		// set asset
		string fileName = data.assets[Random.Range(0, data.assets.Length)];
		string path = "Tilesets/Item/Weapon/" + fileName;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		SetAsset(asset);
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}


	public bool IsRanged (string assetName) {
		return type == "Ranged";
	}


	private void SetStats (string assetName) {
		switch (assetName) {
			case "dagger": damage = "1d4"; break;

			case "quarterstaff": damage = "1d6"; break;

			case "sabre": damage = "1d6"; break;
			case "scimitar": damage = "1d8+1"; break;
			case "katana": damage = "1d10+2"; break;

			case "sword-short": damage = "1d6-1"; break;
			case "sword-long":  damage = "1d8"; break;
			case "sword-great": damage = "2d6"; break;

			case "axe-short": damage = "1d6-1"; break;
			case "axe-long": damage = "1d8"; break;
			case "axe-great": damage = "2d6"; break;

			case "club": damage = "1d6-1"; break;
			case "mace": damage = "1d8"; break;
			case "mace-great": damage = "2d6"; break;


			case "bow-short": damage = "1d4-1"; break;
			case "bow-long": damage = "1d6-1"; break;
			case "bow-great": damage = "1d8"; break;

			case "crossbow-1": damage = "1d4+1"; break;
			case "crossbow-2": damage = "1d4+1"; break;
			
			case "sling-1": damage = "1d4"; break;
			case "sling-2": damage = "1d4"; break;
		}
	}
	
}
