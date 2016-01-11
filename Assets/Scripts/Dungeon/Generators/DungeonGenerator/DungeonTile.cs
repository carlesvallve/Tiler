using UnityEngine;
using System.Collections;


// Tile Types
public enum DungeonTileType {
	EMPTY = 0,
	ROOM = 1,
	WALL = 2,
	CORRIDOR = 3,
	WALLCORNER = 4,
	DOORH = 5,
	DOORV = 6
}


public class DungeonTileNeighbours {
	public DungeonTile n = null;
	public DungeonTile s = null;
	public DungeonTile w = null;
	public DungeonTile e = null;
}


//[System.Serializable]

public class DungeonTile {

	public DungeonTileType id;
	public int x;
	public int y;

	public GameObject obj;

	public DungeonRoom room;

	public Color color = Color.white;
	public Material material;


	public DungeonTile ( DungeonTileType id, int x, int y ) {
		this.id = id;
		this.x = x;
		this.y = y;
	}


	public bool getWalkable () {
		switch (id) {
		case DungeonTileType.ROOM:
			return true;
		case DungeonTileType.CORRIDOR:
			return true;
		case DungeonTileType.DOORH:
		case DungeonTileType.DOORV:
			return true;
		default:
			return false;
		}
	}


	public bool isWall () {
		return 
		id == DungeonTileType.WALL || 
		id == DungeonTileType.WALLCORNER || 
		id == DungeonTileType.DOORH || 
		id == DungeonTileType.DOORV;
	}

	public bool isEmpty () {
		return id == DungeonTileType.EMPTY;
	}
}
