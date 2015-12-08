using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChestGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Chest generation
	// =====================================================

	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxChests = Random.Range(0, 100) <= 60 ? Random.Range(1, (int)(room.tiles.Count * 0.1f)) : 0;

			// place furniture in room
			for (int i = 1; i <= maxChests; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				Chest chest = (Chest)grid.CreateEntity(typeof(Chest), tile.x, tile.y, 0.7f, null) as Chest;
				EntityStates[] states = new EntityStates[] { 
					EntityStates.Closed, EntityStates.Locked  // EntityStates.Open, 
				};
				chest.SetState(states[Random.Range(0, states.Length)]);
				chest.SetRandomItems();
			}
		}
	}

}
