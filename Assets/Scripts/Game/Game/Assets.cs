using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Assets {

	public bool verbose = false;

	public Dictionary<string, Sprite> dungeon = new Dictionary<string, Sprite>();
	public Dictionary<string, Sprite> monster = new Dictionary<string, Sprite>();
	public Dictionary<string, Sprite> item = new Dictionary<string, Sprite>();


	public Assets () {
		// Load dungeon assets
		Sprite[] dungeonAssets = Resources.LoadAll<Sprite>("Tilesets/Dungeon/Images");
		foreach (Sprite sprite in dungeonAssets) { dungeon.Add(sprite.name, sprite); }
		LogAssets(dungeon);

		// Load monster assets
		Sprite[] monsterAssets = Resources.LoadAll<Sprite>("Tilesets/Monster/Images");
		foreach (Sprite sprite in monsterAssets) { monster.Add(sprite.name, sprite); }
		LogAssets(monster);

		// Load item assets
		Sprite[] itemAssets = Resources.LoadAll<Sprite>("Tilesets/Item/Images/Food");
		foreach (Sprite sprite in itemAssets) { item.Add(sprite.name, sprite); }
		LogAssets(monster);
	}


	private void LogAssets (Dictionary<string, Sprite> assetDic) {
		if (!verbose) { return; }

		foreach(KeyValuePair<string,Sprite> asset in assetDic) {
			Debug.Log(asset.Key + ": " + asset.Value);
		}
	}
}
