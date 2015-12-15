using UnityEngine;
using System.Collections;


public class Armour : Item {

	public int armour;

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Armour/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		stackable = false;
		equipmentSlot = "Armour";
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"armour-leather-1", "armour-leather-2", "armour-leather-3", "armour-leather-decorated", "armour-leather-elven",  "armour-leather-studded", 
			"mail-banded",
			"mail-chain-1", "mail-chain-2", "mail-chain-3", 
			"mail-plate-1", "mail-plate-2",
			"mail-ring", 
			"mail-scale", "mail-scale-elven",  
			"mail-splint",
			"robe-blue", "robe-mage", "robe-simple", 
			"skin-animal-1", "skin-animal-2", "skin-lizard", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
	}


	private void SetStats (string assetName) {
		string[] name = assetName.Split('-');
		string type = name[0];
		string quality = name[1];

		switch (type) {
			case "armour":
			case "mail":
			switch (quality) {
				case "leather": armour = 1; break;
				case "banded": armour = 2; break;
				
				case "ring": armour = 3; break;
				case "chain": armour = 4; break;
				case "scale": armour = 5; break;
				case "splint": armour = 6; break;
				case "plate": armour = 9; break;
			}
			break;
			
			case "robe": armour = 0; break;
			case "skin": armour = 0; break;
		}
	}

}
