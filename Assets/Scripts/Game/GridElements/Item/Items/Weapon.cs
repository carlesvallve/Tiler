using UnityEngine;
using System.Collections;


public class Weapon : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/weapon/" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "weapon";
		ammount = 1;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"axe-great", 
			"axe-long", 
			"axe-short", 
			"club", 
			"dagger", 
			"katana", 
			"mace", 
			"mace-great", 
			"quarterstaff", 
			"sabre", 
			"scimitar", 
			"sword-great", 
			"sword-long", 
			"sword-short", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Pickup(Creature creature) {
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/weapon", 0.5f, Random.Range(0.8f, 1.2f));
		}
		
		base.Pickup(creature);
	}
}
