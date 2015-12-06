using UnityEngine;
using System.Collections;


public class Armour : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/Images/Armour/" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "armour";
		ammount = 1;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"boots-brown", 
			"boots-green", 
			"cloak", 
			"gloves", 
			"mail-banded", 
			"mail-leather", 
			"mail-noble", 
			"mail-plate", 
			"mail-ring", 
			"mail-scale", 
			"mail-splint", 
			"robe", 
			"skin",
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Pickup(Creature creature) {
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/armour", 0.7f, Random.Range(0.8f, 1.2f));
		}
		
		base.Pickup(creature);
	}
}
