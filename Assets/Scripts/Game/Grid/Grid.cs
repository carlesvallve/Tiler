using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Grid : MonoSingleton <Grid> {

	public Transform container;

	public GameObject tilePrefab;
	public GameObject barPrefab;
	public GameObject bloodPrefab;
	public GameObject glowPrefab;

	public int width = 16;
	public int height = 16;

	public GridLayers layers;

	public Stair stairUp;
	public Stair stairDown;

	public Player player;


	// =====================================================
	// Initialization
	// =====================================================

	public void InitializeGrid (int width, int height) {
		// reset grid
		ResetGrid();

		// set grid dimensions
		this.width = width;
		this.height = height;

		// initialize grid layers
		layers = new GridLayers () {
			{ typeof(Tile), new GridLayer<Tile> (height, width) },
			{ typeof(Entity), new GridLayer<Entity> (height, width) },
			{ typeof(Creature), new GridLayer<Creature> (height, width) },
		};

		// initialize astar
		InitializeAstar();
	}


	public void ResetGrid () {
		// destroy all previously generated gameobjects except player

		Tile[] tilesToDestroy = container.GetComponentsInChildren<Tile>();
		foreach (Tile tile in tilesToDestroy) {
			// player must persist through all dungeon levels
			if (tile is Player) { continue; } 

			tile.StopAllCoroutines();
			Destroy(tile.gameObject);
		}

		this.width = 0;
		this.height = 0;
	}


	public void InitializeAstar () {
		Cell [,] arr = new Cell[width, height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				Tile tile = GetTile(x, y);
				arr[x, y] = new Cell(x, y, tile != null && tile.IsWalkable());
			}
		}
		
		new Astar(arr);
	}


	public bool IsInsideBounds (int x, int y) {
		if (x < 0 || y < 0 || x > width - 1 || y > height - 1) {
			return false;
		}

		return true;
	}


	// =====================================================
	// Tiles
	// =====================================================

	public Tile CreateTile (System.Type TileType, int x, int y, float scale = 1, Sprite asset = null, bool setInGrid = true) {
		Transform parent = container.Find("Tiles");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = TileType.ToString();

		Tile tile = obj.AddComponent(TileType) as Tile;
		tile.Init(this, x, y, scale, asset);

		if (setInGrid) { SetTile(x, y, tile); }

		return tile;
	}


	public void SetTile (int x, int y, Tile tile) {
		if (!IsInsideBounds(x, y)) { return; }

		// refresh astar walkability
		Astar.instance.walkability[x, y] = (tile != null && !tile.IsWalkable()) ? 1 : 0;

		layers.Set<Tile>(y, x, tile);
	}


	public Tile GetTile (int x, int y) {
		if (!IsInsideBounds(x, y)) { return null; }
		return layers.Get<Tile>(y, x);
	}

	public Tile GetTile (Vector3 pos) {
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);
		if (!IsInsideBounds(x, y)) { return null; }

		return layers.Get<Tile>(y, x);
	}


	public bool TileIsOpaque (int x, int y) {
		if (!IsInsideBounds(x, y)) {
			return true; 
		}

		Tile tile = GetTile(x, y);
		if (tile == null) { return true; }

		return tile.IsOpaque();
	}

	
	// =====================================================
	// Entities
	// =====================================================

	public Entity CreateEntity (System.Type EntityType, int x, int y, float scale = 1, Sprite asset = null, bool setInGrid = true) {
		Transform parent = container.Find("Entities");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = EntityType.ToString();

		Entity entity = obj.AddComponent(EntityType) as Entity;
		entity.Init(this, x, y, scale, asset);

		if (setInGrid) { SetEntity(x, y, entity); }

		return entity;
	}


	public void SetEntity (int x, int y, Entity entity) {
		if (!IsInsideBounds(x, y)) { return; }

		// refresh astar walkability
		Astar.instance.walkability[x, y] = (entity != null && !entity.IsWalkable()) ? 1 : 0;

		layers.Set<Entity>(y, x, entity);
	}


	public Entity GetEntity (int x, int y) {
		if (!IsInsideBounds(x, y)) { return null; }
		return layers.Get<Entity>(y, x);
	}


	// =====================================================
	// Creatures
	// =====================================================

	public Creature CreateCreature (System.Type CreatureType, int x, int y, float scale = 1, Sprite asset = null, bool setInGrid = true) {
		Transform parent = (CreatureType == typeof(Player)) ? container.Find("Player") : container.Find("Creatures");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = CreatureType.ToString();

		Creature creature = obj.AddComponent(CreatureType) as Creature;

		obj = (GameObject)Instantiate(barPrefab);
		obj.transform.SetParent(creature.transform.Find("Sprites"), false);
		obj.name = "Bar";
		creature.bar = obj.GetComponent<HpBar>();

		creature.Init(this, x, y, scale,  asset);

		if (setInGrid) { SetCreature(x, y, creature); }

		return creature;
	}


	public void SetCreature (int x, int y, Creature creature) {
		if (!IsInsideBounds(x, y)) { return; }

		// refresh astar walkability
		Astar.instance.walkability[x, y] = (creature != null && !creature.IsWalkable()) ? 1 : 0;

		layers.Set<Creature>(y, x, creature);
	}


	public Creature GetCreature (int x, int y) {
		if (!IsInsideBounds(x, y)) { return null; }
		return layers.Get<Creature>(y, x);
	}


	// =====================================================
	// Fx
	// =====================================================

	public Blood CreateBlood (Vector3 pos, int maxParticles, Color color) {
		Transform parent = container.Find("Fx");

		GameObject obj = (GameObject)Instantiate(bloodPrefab);
		obj.transform.SetParent(parent, false);
		obj.name = "Blood";

		Blood blood = obj.GetComponent<Blood>();
		blood.Init(pos, maxParticles, color);

		return blood;
	}
	

	public Glow CreateGlow (Vector3 pos, int maxParticles, Color color) {
		Transform parent = container.Find("Fx");

		GameObject obj = (GameObject)Instantiate(glowPrefab);
		obj.transform.SetParent(parent, false);
		obj.name = "Glow";

		Glow glow = obj.GetComponent<Glow>();
		glow.Init(pos, maxParticles, color);

		return glow;
	}


	// =====================================================
	// Neighbour methods
	// =====================================================

	public List<Tile> GetNeighbours (int x, int y, bool addCenterTile = false) {
		List<Tile> neighbours = new List<Tile>() {
			GetTile(x + 0, y - 1),
			GetTile(x + 1, y - 1),
			GetTile(x + 1, y + 0),
			GetTile(x + 1, y + 1),
			GetTile(x + 0, y + 1),
			GetTile(x - 1, y + 1),
			GetTile(x - 1, y + 0),
			GetTile(x - 1, y - 1),
		};

		if (addCenterTile) {
			neighbours.Add(GetTile(x, y));
		}

		return neighbours;
	}


	public List<Tile> GetPassableNeighbours (int x, int y, bool addCenterTile = false) {
		List<Tile> tiles = GetNeighbours(x, y, addCenterTile);
		
		List<Tile> neighbours = new List<Tile>();
		foreach (Tile tile in tiles) {
			if (tile != null && tile.IsPassable()) {
				neighbours.Add(tile);
			}
		}

		return neighbours;
	}


	public List<Tile> GetNonOccupiedNeighbours (int x, int y, bool addCenterTile = false) {
		List<Tile> tiles = GetNeighbours(x, y, addCenterTile);

		List<Tile> neighbours = new List<Tile>();
		foreach (Tile tile in tiles) {
			if (tile != null && !tile.IsOccupied()) {
				neighbours.Add(tile);
			}
		}

		return neighbours;
	}

}

