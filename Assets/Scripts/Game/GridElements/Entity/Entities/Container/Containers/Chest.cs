﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Chest : Container {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		this.assetType = GetRandomAssetName();
		asset = Resources.Load<Sprite>("Tilesets/Container/" + assetType + "-closed");

		base.Init(grid, x, y, scale, asset);
		
		breakable = false;
		maxItems = Random.Range(1, 4);
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { "chest" };
		return arr[Random.Range(0, arr.Length)];
	}


	protected override System.Type GetRandomItemType () {
		// Pick a weighted random item type
		return Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
			{ typeof(Armour), 	10 },
			{ typeof(Weapon), 	10 },
			{ typeof(Treasure), 25 },
			{ typeof(Book), 	10 },
			{ typeof(Food), 	0 },
			{ typeof(Potion), 	10 },
		});
	}

}