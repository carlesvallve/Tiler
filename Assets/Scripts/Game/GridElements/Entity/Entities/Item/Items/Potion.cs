﻿using UnityEngine;
using System.Collections;


public class Potion : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/Potion/potion-" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "potion";
		ammount = 1;
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"blue", 
			"cyan", 
			"red", 
			"white",  
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void Pickup(Creature creature) {
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/potion", 0.4f, Random.Range(0.8f, 1.2f));
		}
		
		base.Pickup(creature);
	}


	public override void Use (Creature creature) {
		// food heals 2d6 hp for now
		int hp = Dice.Roll("2d6"); //Random.Range(1, 5);
		creature.UpdateHp(hp);
		 
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/potion", 0.4f, Random.Range(0.8f, 1.2f));
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

}
