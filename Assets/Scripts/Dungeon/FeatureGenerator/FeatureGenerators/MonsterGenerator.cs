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
		// generate dictionary with monster rarities
		Dictionary<string, double> rarities = GenerateRarityTable();

		// for each room in the dungeon
		for (int n = 0; n < dungeonGenerator.rooms.Count; n++) {
			// decide how many monsters of a type are in the room
			DungeonRoom room = dungeonGenerator.rooms[n];

			// get max monsters, relative to max number of tiles in the room
			int maxMonsters = 0;
			int r = Random.Range(1, 100);
			if (r <= 50) {
				int maxTiles = Mathf.RoundToInt(room.tiles.Count * 0.15f);
				maxMonsters = Random.Range(1, maxTiles);
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


	private Dictionary<string, double> GenerateRarityTable () {
		Dictionary<string, double> rarities = new Dictionary<string, double>();

		// iterate Gamedata.monsters dictionary and add key/rarity pairs
		foreach (KeyValuePair<string, MonsterData> entry in GameData.monsters) {
			// use entry.Value.rarity once we setup final monster spreadsheet
			// for now rariry depends on monster overall dangerousness
			
			int dlevel = Dungeon.instance.currentDungeonLevel;
			int rarity = 100 + (dlevel * 3) - ((entry.Value.hp + entry.Value.gdr) * 2);

			// cap rarity so weak monsters dont appear on high dungeon levels
			int capLevel = 100;
			if (rarity > capLevel) { rarity = 0; }

			rarities.Add(entry.Key, rarity);

			Debug.Log (entry.Key + " " + rarity + " / " + capLevel);
		}

		return rarities;
	}
}
