using UnityEngine;
using System.Collections;


public class Cloak : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Cloak/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		equippable = true;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"cloak-blue", "cloak-brown", "cloak-gray", "cloak-purple", 
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
