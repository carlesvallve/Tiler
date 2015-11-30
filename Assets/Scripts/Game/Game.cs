using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


public class Game : MonoBehaviour {

	public static Assets assets;

	//public Grid grid;
	public Dungeon dungeon;
	public Grid grid;

	void Start () {
		assets = new Assets();

		//grid = GetComponent<Grid>();
		dungeon = Dungeon.instance; //GetComponent<Dungeon>();
		grid = Grid.instance;

		InitGame();
	}


	private void InitGame () {
		// Generate dungeon level
		dungeon.GenerateDungeon();

		// Render dungeon in grid
		dungeon.RenderDungeon();
	}

}







