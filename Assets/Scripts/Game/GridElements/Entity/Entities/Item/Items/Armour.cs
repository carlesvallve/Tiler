using UnityEngine;
using System.Collections;


public class Armour : Item {

	public int ac;
	public int gdr;
	public int sh;
	public int ev;
	public int encumberness;
	public int weight;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		InitializeStats(id);
	}

	public void InitializeStats (string id) {
		// assign props from csv
		ArmourData data = GameData.armours[id];

		this.id = data.id;
		this.type = data.type;
		this.adjective = data.adjective;

		this.ac = data.ac;
		this.gdr = data.gdr;
		this.sh = data.sh;
		this.ev = data.ev;
		this.encumberness = data.encumberness;
		this.weight = data.weight;

		// set asset
		string fileName = data.assets[Random.Range(0, data.assets.Length)];
		string path = "Tilesets/Item/Body/" + type + "/" + fileName;
		this.asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		SetAsset(asset);

		// extra props
		this.walkable = true;
		this.stackable = false;
		this.equipmentSlot = data.type;
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
	}

}
