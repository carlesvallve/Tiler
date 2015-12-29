using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;


public class MonsterGenerator : DungeonFeatureGenerator {

	// =====================================================
	// Monster generation
	// =====================================================

	// TODO:
	// Implement a balanced level spawning algorithm
	// Monsters should be spawned by affinity groups, relative to the dungeon level 
	
	public void GenerateSingle (string id) {
		Random.seed = Dungeon.seed;
		
		int roomId = Grid.instance.GetTile(Grid.instance.player.x, Grid.instance.player.y).roomId;
		DungeonRoom room = dungeonGenerator.rooms[roomId];

		Tile tile = GetFreeTileOnRoom(room, 0);
		if (tile == null) { 
			Debug.LogError ("Monster could not be placed anywhere!"); 
			return;
		}

		// create the monster and initialize it by given id
		grid.CreateCreature(typeof(Monster), tile.x, tile.y, 0.7f, null, id);
	}


	public override void Generate () {
		Random.seed = Dungeon.seed;

		// generate monster rarity table dictionary
		Dictionary<string, double> rarities = GameData.GenerateMonsterRarityTable();

		// for each room in the dungeon
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {
			// decide how many monsters of a type are in the room
			DungeonRoom room = dungeonGenerator.rooms[n];

			// get max monsters in level
			int maxMonsters = 0;

			if (dungeonGenerator.rooms.Count == 1) {
				// calculate max monsters for single room levels
				int maxTiles = Mathf.RoundToInt(room.tiles.Count * 0.05f);
				maxMonsters = Random.Range(maxTiles / 2, maxTiles);
			} else {
				// calculate max monsters relative to max number of tiles in each room
				int r = Random.Range(1, 100);
				if (r <= 40) {
					int maxTiles = Mathf.RoundToInt(room.tiles.Count * 0.1f);
					maxMonsters = Random.Range(1, maxTiles);
				}
			}

			// continue if this room has no monsters
			if (maxMonsters == 0) { continue; }

			for (int i = 1; i <= maxMonsters; i ++) {
				Tile tile = GetFreeTileOnRoom(room, 0);
				if (tile == null) { continue; }

				// get a monster random entry from rarity table dictionary
				string id = Dice.GetRandomStringFromDict(rarities);

				// create the monster and initialize it by id
				grid.CreateCreature(typeof(Monster), tile.x, tile.y, 0.7f, null, id);
			}
		}
	}
}
