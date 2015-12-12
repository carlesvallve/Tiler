using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ContainerGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Container generation
	// =====================================================

	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxContainers = Random.Range(0, 100) <= 60 ? Random.Range(1, (int)(room.tiles.Count * 0.1f)) : 0;

			// place furniture in room
			for (int i = 1; i <= maxContainers; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// Pick a weighted random container type
				System.Type containerType = Dice.GetRandomTypeFromDict(new Dictionary<System.Type, double>() {
					{ typeof(Chest), 	1 },
					{ typeof(Barrel), 	1 },
					{ typeof(Vase), 	1 },
				});

				Container container = (Container)grid.CreateEntity(containerType, tile.x, tile.y, 0.7f, null) as Container;
				
				EntityStates[] states = new EntityStates[] { EntityStates.Closed, EntityStates.Locked };
				container.SetState(states[Random.Range(0, states.Length)]);
				
				container.SetItems();
			}
		}
	}

}
