using UnityEngine;
using System.Collections;


public class Armour : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Armour/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		equippable = true;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"armour-leather-1", "armour-leather-2", "armour-leather-3", "armour-leather-decorated", "armour-leather-elven",  "armour-leather-studded", 
			"mail-banded",
			"mail-chain-1", "mail-chain-2", "mail-chain-3", 
			"mail-plate-1", "mail-plate-2",
			"mail-ring", 
			"mail-scale-1", "mail-scale-eleven",  
			"mail-splint",
			"robe-blue", "robe-mage", "robe-simple", 
			"sin-animal-1", "sin-animal-2", "sin-lizard", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Pickup (Creature creature) {
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/armour", 0.9f, Random.Range(0.8f, 1.2f));
		}
		
		base.Pickup(creature);
	}

}
