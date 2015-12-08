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
	
	public void GenerateSingle () {
		DungeonRoom room = dungeonGenerator.rooms[Grid.instance.player.roomId];
		Tile tile = GetFreeTileOnRoom(room, 0);

		List<System.Type> types = GetMonsterTypes();
		System.Type monsterType = types[Random.Range(0, types.Count)];

		grid.CreateCreature(monsterType, tile.x, tile.y, 0.7f, null);
	}


	public override void Generate () {
		// for each room in the dungeon
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {
			// decide how many monsters of a type are in the room
			DungeonRoom room = dungeonGenerator.rooms[n];
			int maxMonsters = Random.Range(0, 100) <= 60 ? Random.Range(1, (int)(room.tiles.Count * 0.15f)) : 0;
			if (maxMonsters == 0) { return; }

			// decide the type of monsters
			List<System.Type> types = GetMonsterTypes();
			System.Type monsterType = types[Random.Range(0, types.Count)];

			// create a monster in a random available tile inside the room
			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }
				
				grid.CreateCreature(monsterType, tile.x, tile.y, 0.7f, null);
			}
		}
	}


	private List<System.Type> GetMonsterTypes () {
		// Pick a random creature type
		List<System.Type> types = new List<System.Type>() { 
			// animals
			typeof(Bat), 
			typeof(Bear), 
			typeof(Cat), 
			typeof(Chicken), 
			typeof(Cow), 
			typeof(Crab), 
			typeof(Dog),
			typeof(Duck),
			typeof(Fly),   
			typeof(Goat),
			typeof(Goose),  
			typeof(Horse), 
			typeof(Lion), 
			typeof(Mouse),
			typeof(Pig), 
			typeof(Pigeon),  
			typeof(Sheep),
			typeof(Scorpion),
			typeof(Turkey), 
			typeof(Wolf),
			 
			// humanoids
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

		return types;
	}

}
