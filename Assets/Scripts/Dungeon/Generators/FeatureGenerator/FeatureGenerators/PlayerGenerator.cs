using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Player generation
	// =====================================================

	// note: player must be generated before monsters for them to be able to listen to player events

	public override void GenerateAtPos (int x, int y) {
		if (grid.player == null) {
			grid.player = grid.CreateCreature(typeof(Player), x, y, 0.7f, null) as Player;
			Camera.main.transform.position = new Vector3(grid.player.x, grid.player.y, -10);
		} else {
			grid.player.LocateAtCoords(x, y);
			Camera2D.instance.CenterCamera(grid.player, false);
		}
	}

}
