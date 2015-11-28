using UnityEngine;
using System.Collections;

public struct Sprites {
	public Sprite[] dungeon;
	public Sprite[] monster;
}

public class Game : MonoBehaviour {

	public Sprites sprites;


	void Awake () {
		LoadSprites();
	}


	private void  LoadSprites () {
		sprites = new Sprites();
		sprites.dungeon = Resources.LoadAll<Sprite>("Tilesets/Dungeon/Images");
		sprites.monster = Resources.LoadAll<Sprite>("Tilesets/Monster/Images");

		print ("DUNGEON ASSETS");
		foreach (Sprite sprite in sprites.dungeon) {
			print ("    " + sprite.name);
		}

		print ("MONSTER ASSETS");
		foreach (Sprite sprite in sprites.monster) {
			print ("    " + sprite.name);
		}
	}
}
