using UnityEngine;
using System.Collections;


public class Shield : Item {

	public int defense;
	public int weight;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		InitializeStats(id);
	}


	public void InitializeStats (string id) {
		// assign props from csv
		ShieldData data = GameData.shields[id];

		this.id = data.id;
		this.type = data.type;
		this.adjective = data.adjective;
		this.rarity = data.rarity;

		this.defense = data.defense;
		this.weight = data.weight;

		// set asset
		string fileName = data.assets[Random.Range(0, data.assets.Length)];
		string path = "Tilesets/Item/Shield/" + fileName;
		this.asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		SetAsset(asset);

		// extra props
		this.walkable = true;
		this.stackable = false;
		this.equipmentSlot = "Shield";
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}

}
