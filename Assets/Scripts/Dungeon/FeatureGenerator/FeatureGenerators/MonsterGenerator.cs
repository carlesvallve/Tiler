using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MonsterGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Monster generation
	// =====================================================

	// TODO: 
	// Implement a balanced level spawning algorithm
	// Monsters should be spawned by affinity groups, relative to the dungeon level 
	

	public override void Generate () {
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {

			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxMonsters = Random.Range(0, 100) <= 40 ? Random.Range(1, (int)(room.tiles.Count * 0.1f)) : 0;

			//Color color = new Color (Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			//PaintRoom(room, color);

			// Pick a random creature type
			List<System.Type> types = new List<System.Type>() { 
				typeof(Caveman), 
				typeof(Centaur), 
				typeof(Circus), 
				typeof(Demon), 
				typeof(Giant), 
				typeof(Goblin), 
				typeof(Goblin), 
				typeof(Gorilla), 
				typeof(KnightDark), 
				typeof(KnightLight), 
				typeof(Merchant), 
				typeof(Minotaur), 
				typeof(Monkey), 
				typeof(Orangutan), 
				typeof(Peasant), 
				typeof(Pirate), 
				typeof(Ratman), 
				typeof(Satir), 
				typeof(Snakeman), 
				typeof(Troll), 
				typeof(Vampire), 
				typeof(Viking), 
				typeof(Zombie), 
			};

			System.Type monsterType = types[Random.Range(0, types.Count)];

			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				grid.CreateCreature(monsterType, tile.x, tile.y, 0.7f, null);

			}
		}
	}

}
