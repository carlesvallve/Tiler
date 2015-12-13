using UnityEngine;
using System.Collections;


public class Boots : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		string path = "Tilesets/Item/Body/Boots/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.Log(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		equippable = true;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"boots-brown", "boots-green", "boots-iron", "boots-striped", 
			"shoes-leather-1", "shoes-leather-2", 
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
