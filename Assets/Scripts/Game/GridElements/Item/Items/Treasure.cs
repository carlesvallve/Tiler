using UnityEngine;
using System.Collections;


public class Treasure : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/Treasure/" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "treasure";
		ammount = Random.Range(1, 10);
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { "gold" };

		return arr[Random.Range(0, arr.Length)];
	}

	public override void Pickup(Creature creature) {
		creature.stats.gold += ammount;

		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/treasure", 0.15f, Random.Range(0.5f, 1.0f));
			Speak("+" + ammount, Color.yellow);
		}

		base.Pickup(creature);
	}
}
