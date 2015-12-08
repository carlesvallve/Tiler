using UnityEngine;
using System.Collections;


public class Food : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/Food/" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "food";
		ammount = 1;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"apricot", "banana", "bread", "meat", "strawberry" 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Pickup(Creature creature) {
		// food heals hp for now
		int hp = Random.Range(1, 5);
		if (creature.stats.hp < creature.stats.hpMax) {
			creature.UpdateHp(hp);
		} else {
			hp = 0;
		}

		if (creature.visible) {
			if (hp > 0) {
				sfx.Play("Audio/Sfx/Item/food", 0.5f, Random.Range(0.8f, 1.2f));
				Speak("+" + hp, Color.cyan);
			} else {
				Speak("Full", Color.cyan);
				return;
			}
		}

		base.Pickup(creature);
	}


	/*public override void Drop(Creature creature, int x, int y) {
		if (creature.visible) {	
			sfx.Play("Audio/Sfx/Item/food", 0.5f, Random.Range(0.8f, 1.2f));
		}

		base.Drop(creature, x, y);
	}*/
}
