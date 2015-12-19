using UnityEngine;
using System.Collections;


public class Book : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		string path = "Tilesets/Item/Book/" + GetRandomAssetName();
		asset = Resources.Load<Sprite>(path);
		if (asset == null) { Debug.LogError(path); }

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
