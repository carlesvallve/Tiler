using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StairGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Stair generation
	// =====================================================

	public override void Generate () {
		Tile tile = null;

		grid.stairUp = null;
		grid.stairDown = null;

		// locate ladderUp so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(0);
		if (tile != null) {
			grid.stairUp = (Stair)grid.CreateEntity(typeof(Stair), tile.x, tile.y, 0.8f, null) as Stair;
			grid.stairUp.SetDirection(-1);
			grid.stairUp.state = dungeon.currentDungeonLevel == 0 ? EntityStates.Locked : EntityStates.Open;
		}

		// locate ladderDown so it has no entities on 1 tile radius
		tile = GetFreeTileOnGrid(0);
		if (tile != null) {
			grid.stairDown = (Stair)grid.CreateEntity(typeof(Stair), tile.x, tile.y, 0.8f, null) as Stair;
			grid.stairDown.SetDirection(1);
		}
	}

}
