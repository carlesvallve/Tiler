using UnityEngine;
using System.Collections;


public class Equipment : Item {

	/*public int attack;
	public string damage;
	public int range;
	public int hands;
	public int weight;*/

	//public string id;
	//public string[] assets;

	//public string type;
	//public string subtype;
	//public int rarity;

	public int attack;
	public int defense;
	public string damage;
	public int armour;
	public int range;
	public int hands;
	public int weight;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		InitializeStats(id);
	}


	public void InitializeStats (string id) {
		// assign props from csv
		EquipmentData data = GameData.equipments[id];

		this.id = data.id;
		this.type = data.type;
		this.subtype = data.subtype;
		this.rarity = data.rarity;

		this.attack = data.attack;
		this.defense = data.defense;
		this.damage = data.damage;
		this.armour = data.armour;
		this.range = data.range;
		this.hands = data.hands;
		this.weight = data.weight;

		// set asset
		//string str = ""; foreach(string myasset in data.assets) { str += myasset + " "; }; Debug.Log(str);

		string fileName = data.assets[Random.Range(0, data.assets.Length)];
		string path = "Tilesets/Equipment/" + this.type + "/" + fileName;
		this.asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }
		SetAsset(asset);

		// extra props
		this.walkable = true;
		this.stackable = false;
		this.equipmentSlot = this.type; //"Weapon";
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/weapon", 0.6f, Random.Range(0.8f, 1.2f));
	}

}

