using UnityEngine;
using System.Collections;

using AssetLoader;


public class Equipment : Item {

	public int attack;
	public int defense;
	public string damage;
	public int armour;
	public int range;
	public int hands;
	public int weight;

	//public string tileType;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		base.Init(grid, x, y, scale, asset);
		SetImages(scale, Vector3.zero, 0.04f);

		this.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		//this.img.color = color;

		InitializeStats(id);

		AdjustAspect();
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
		this.rarity = data.rarity;

		this.attack = data.attack;
		this.defense = data.defense;
		this.damage = data.damage;
		this.armour = data.armour;
		this.range = data.range;
		this.hands = data.hands;
		this.weight = data.weight;

		// set asset
		this.asset = LoadAsset(data);
		SetAsset(this.asset);

		// extra props
		this.walkable = true;
		this.stackable = false;
		this.equipmentSlot = this.type;

		this.name = this.type;
	}


	private Sprite LoadAsset (EquipmentData data) {
		// set asset with alternative old way (boots / gloves / cloak)
		if (type == "Boots" || type == "Gloves" || type == "Cloak") {
			/*string fileName = data.assets[Random.Range(0, data.assets.Length)];
			string path = "Tilesets/Equipment/" + this.type + "/" + fileName;
			
			Sprite asset = Resources.Load<Sprite>(path);
			if (asset == null) { Debug.LogError(path); }*/

			Sprite asset = null; //Assets.GetAsset("Equipment/" + type + "/" + fileName);

			return asset;
		}

		// set asset form AssetManager
		return null; //AssetManager.LoadEquipmentPart(type, subtype);
	}


	private void AdjustAspect () {
		Vector3 scale = img.transform.localScale;
		Vector3 pos = Vector3.zero;

		switch (type) {
			case "Armour":
				pos = new Vector3(0, -0.1f, 0);
				if (subtype == "Robe") { 
					scale = new Vector3(scale.x, scale.y * 0.75f, scale.z); 
					pos = new Vector3(0, 0.05f, 0);
				}
				break;
			case "Weapon":
				pos = new Vector3(0.25f, 0, 0);
				break;
			case "Shield":
				pos = new Vector3(-0.25f, 0, 0);
				break;
			case "Head":
				pos = new Vector3(0, -0.5f, 0);
				break;
		}

		SetImages(scale, pos, 0.035f);
	}


	public override void PlaySoundUse () {
		if (equipmentSlot == "Weapon" || equipmentSlot == "Shield") {
			sfx.Play("Audio/Sfx/Item/weapon", 0.4f, Random.Range(0.8f, 1.2f));
		} else {
			sfx.Play("Audio/Sfx/Item/armour-equip", 0.8f, Random.Range(0.8f, 1.2f));
		}
	}
}

