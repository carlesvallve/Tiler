using UnityEngine;
using System.Collections;

using AssetLoader;


public class Book : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		Sprite[] assets = Assets.GetCategory("Item/Book");
		asset = assets[Random.Range(0, assets.Length)];

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);
	}


	protected override string GetRandomAssetName () {
		string[] arr = new string[] { 
			"book-black", 
			"book-blue", 
			"book-red", 
			"book-white", 
			"scroll", 
		};

		return arr[Random.Range(0, arr.Length)];
	}


	public override void PlaySoundPickup () {
		sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
	}

	public override void PlaySoundUse () {
		sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
	}
	
}
