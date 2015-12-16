using UnityEngine;
using System.Collections;


public class Armour : Item {

	//public int armour;

	public string type;
	public string adjective;

	public int ac;
	public int gdr;
	public int sh;
	public int ev;
	public int encumberness;
	public int weight;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		walkable = true;
		stackable = false;

		//equipmentSlot = "Armour";

		InitializeStats(id);
	}

	public void InitializeStats (string id) {
		// assign props from csv
		ArmourData data = GameData.armours[id];

		id = data.id;
		type = data.type;
		adjective = data.adjective;

		ac = data.ac;
		gdr = data.gdr;
		sh = data.sh;
		ev = data.ev;
		encumberness = data.encumberness;
		weight = data.weight;

		// set asset
		string fileName = data.assets[Random.Range(0, data.assets.Length)];
		string path = "Tilesets/Item/Body/" + type + "/" + fileName;
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		SetAsset(asset);
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
	}

}
