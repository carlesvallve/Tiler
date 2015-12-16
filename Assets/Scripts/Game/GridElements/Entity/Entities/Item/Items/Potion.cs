using UnityEngine;
using System.Collections;


public class Potion : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		string path = "Tilesets/Item/Potion/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		consumable = true;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"potion-blue", 
			"potion-cyan", 
			"potion-red", 
			"potion-white",  
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Use (Creature creature) {
		// food heals 2d6 hp for now
		int hp = Dice.Roll("2d6");
		creature.UpdateHp(hp);
		 
		if (creature.visible) {
			PlaySound();
			Speak("+" + hp, Color.cyan);
		} 
	}


	public override bool CanUse (Creature creature) {
		if (creature.stats.hp < creature.stats.hpMax) {
			return true;
		}

		Speak("Full", Color.cyan);
		return false;
	}


	public override void PlaySound () {
		sfx.Play("Audio/Sfx/Item/potion", 0.4f, Random.Range(0.8f, 1.2f));
	}

}
