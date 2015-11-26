using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridPrefabs {

	public TilePrefabs[] tilePrefabs;
	public Dictionary<TileTypes, GameObject> tiles = new Dictionary<TileTypes, GameObject>();

	public ObstaclePrefabs[] obstaclePrefabs;
	public Dictionary<ObstacleTypes, GameObject> obstacles = new Dictionary<ObstacleTypes, GameObject>();

	public DoorPrefabs[] doorPrefabs;
	public Dictionary<DoorTypes, GameObject> doors = new Dictionary<DoorTypes, GameObject>();

	public LadderPrefabs[] ladderPrefabs;
	public Dictionary<LadderTypes, GameObject> ladders = new Dictionary<LadderTypes, GameObject>();

	public ItemPrefabs[] itemPrefabs;
	public Dictionary<ItemTypes, GameObject> items = new Dictionary<ItemTypes, GameObject>();

	public PlayerPrefabs[] playerPrefabs;
	public Dictionary<PlayerTypes, GameObject> players = new Dictionary<PlayerTypes, GameObject>();


	public void Init () {
		// create tiles dictionary: tile prefabs will be accessible by type key
		for (int i = 0; i < tilePrefabs.Length; i++) {
			tiles.Add(tilePrefabs[i].type, tilePrefabs[i].prefab);
		}

		// create obstacles dictionary: obstacle prefabs will be accessible by type key
		for (int i = 0; i < obstaclePrefabs.Length; i++) {
			obstacles.Add(obstaclePrefabs[i].type, obstaclePrefabs[i].prefab);
		}

		// create doors dictionary: door prefabs will be accessible by type key
		for (int i = 0; i < doorPrefabs.Length; i++) {
			doors.Add(doorPrefabs[i].type, doorPrefabs[i].prefab);
		}

		// create ladders dictionary: ladder prefabs will be accessible by type key
		for (int i = 0; i < ladderPrefabs.Length; i++) {
			ladders.Add(ladderPrefabs[i].type, ladderPrefabs[i].prefab);
		}

		// create items dictionary: item prefabs will be accessible by type key
		for (int i = 0; i < itemPrefabs.Length; i++) {
			items.Add(itemPrefabs[i].type, itemPrefabs[i].prefab);
		}

		// create players dictionary: player prefabs will be accessible by type key
		for (int i = 0; i < playerPrefabs.Length; i++) {
			players.Add(playerPrefabs[i].type, playerPrefabs[i].prefab);
		}

		//Debug.Log("Grid prefabs had been initialized: " + tiles);
	}

}
