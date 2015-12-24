using UnityEngine;
using System.Collections;


public class Equipment : Item {

	public int attack;
	public int defense;
	public string damage;
	public int armour;
	public int range;
	public int hands;
	public int weight;

	public string tileType;



	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		this.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		//this.img.color = color;

		InitializeStats(id);
	}


	public void InitializeStats (string id) {
		if (!GameData.equipments.ContainsKey(id)) {
			Debug.LogError(id + " key not present in GameData.equipments dictionary!");
		}

		// assign props from csv
		EquipmentData data = GameData.equipments[id];

		this.id = data.id;
		this.type = data.type;
		this.subtype = data.subtype;
		this.rarity = 100; //data.rarity;

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
		this.equipmentSlot = this.type;

		this.name = this.type;
	}


	public override void PlaySoundUse () {
		if (equipmentSlot == "Weapon" || equipmentSlot == "Shield") {
			sfx.Play("Audio/Sfx/Item/weapon", 0.4f, Random.Range(0.8f, 1.2f));
		} else {
			sfx.Play("Audio/Sfx/Item/armour-equip", 0.8f, Random.Range(0.8f, 1.2f));
		}
		
	}

}

