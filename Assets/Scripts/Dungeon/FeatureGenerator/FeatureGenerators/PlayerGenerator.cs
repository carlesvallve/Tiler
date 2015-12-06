using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Player generation
	// =====================================================

	// note: player must be generated before monsters for them to be able to listen to player events

	// TODO: 
	// player must be created by the parameters user chose when making his unique starting player
	// player should be rendered according to whatever equipment he is currently wearing
	

	public override void GenerateAtPos (int x, int y) {
		if (grid.player == null) {
			Sprite asset = Resources.Load<Sprite>("Tilesets/Monster/adventurer");
			grid.player = grid.CreateCreature(typeof(Player), x, y, 0.8f, asset) as Player;
			Camera.main.transform.position = new Vector3(grid.player.x, grid.player.y, -10);
		} else {
			grid.player.LocateAtCoords(x, y);
			grid.player.CenterCamera(false);
		}
	}

}
