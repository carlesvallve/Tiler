using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MonsterGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Monster generation
	// =====================================================

	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxMonsters = Random.Range(0, 100) <= 35 ? Random.Range(1, (int)(room.tiles.Count * 0.1f)) : 0;

			//Color color = new Color (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			//PaintRoom(room, color);

			// Pick a random creature type
			List<System.Type> types = new List<System.Type>() { typeof(Goblin), typeof(Demon) };
			System.Type monsterType = types[0];

			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				//Monster monster = (Monster)grid.CreateCreature(monsterType, tile.x, tile.y, 0.8f) as Monster;
				grid.CreateCreature(monsterType, tile.x, tile.y, 0.8f);

			}
		}
	}

}
