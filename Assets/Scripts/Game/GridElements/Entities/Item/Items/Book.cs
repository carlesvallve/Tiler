using UnityEngine;
using System.Collections;


public class Book : Item {

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		asset = Resources.Load<Sprite>("Tilesets/Item/Images/Book/" + GetRandomAssetName());

		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		typeId = "book";
		ammount = 1;
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


	public override void Pickup(Creature creature) {
		if (creature.visible) {
			sfx.Play("Audio/Sfx/Item/book", 0.15f, Random.Range(0.8f, 1.2f));
		}
		
		base.Pickup(creature);
	}
}
