using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Assets {

	public Dictionary<string, Sprite> dungeon = new Dictionary<string, Sprite>();
	public Dictionary<string, Sprite> monster = new Dictionary<string, Sprite>();


	public Assets () {
		// Load dungeon assets
		Sprite[] dungeonAssets = Resources.LoadAll<Sprite>("Tilesets/Dungeon/Images");
		foreach (Sprite sprite in dungeonAssets) {
			dungeon.Add(sprite.name, sprite);
		}

		foreach(KeyValuePair<string,Sprite> asset in dungeon) {
			Debug.Log(asset.Key + ": " + asset.Value);
		}

		// Load monster assets
		Sprite[] monsterAssets = Resources.LoadAll<Sprite>("Tilesets/Monster/Images");
		foreach (Sprite sprite in monsterAssets) {
			monster.Add(sprite.name, sprite);
		}

		foreach(KeyValuePair<string,Sprite> asset in monster) {
			Debug.Log(asset.Key + ": " + asset.Value);
		}
	}
}
