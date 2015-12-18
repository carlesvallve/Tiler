using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Barrel : Container {


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		this.assetType = GetRandomAssetName();
		asset = Resources.Load<Sprite>("Tilesets/Container/" + assetType + "-closed");

		base.Init(grid, x, y, scale, asset);
		
		breakable = true;
		maxItems = 1;
	}

	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"barrel", 
			"barrel-water",
		};

		return arr[Random.Range(0, arr.Length)];
	}


	protected override System.Type GetRandomItemType () {
		// Pick a weighted random item type
		return Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
			{ typeof(Equipment), 	100 },
			{ typeof(Treasure), 	20 },
			{ typeof(Book), 		10 },
			{ typeof(Food), 		10 },
			{ typeof(Potion), 		10 },
		});
	}


	

}
