using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour {

	public Assets assets;


	void Awake () {
		assets = new Assets();
		InitGame();
	}


	private void InitGame () {
		/* grid
			- layer 1 <Tile> 
			- layer 2 <Tile> foor overlay -> carpets, traps, blood, items
			- layer 3 <Tile> walls, doors, entities


			- Tile (type, x, y)
				-Entity ()
					- Creature ()
						- Humanoid ()


			- we instantiate a prefab associated to the class
			- all prefabs have a gameObject + sprite child
			- we assign the sprite by name key from assets dictionary 
		*/
	}
}
