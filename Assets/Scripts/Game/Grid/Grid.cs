using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Grid : MonoSingleton <Grid> {

	public Transform container;

	public GameObject tilePrefab;
	public GameObject entityPrefab;
	public GameObject creaturePrefab;

	public int width = 16;
	public int height = 16;

	public GridLayers layers;

	public Stair stairUp;
	public Stair stairDown;

	public Player player;
	public List<Creature> monsters;


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


	public void ResetGrid () {
		// destroy all previously generated gameobjects

		Tile[] tilesToDestroy = container.GetComponentsInChildren<Tile>();
		foreach (Tile tile in tilesToDestroy) {
			Destroy(tile.gameObject);
		}

		Entity[] entitiesToDestroy = container.GetComponentsInChildren<Entity>();
		foreach (Entity entity in entitiesToDestroy) {
			Destroy(entity.gameObject);
		}

		this.width = 0;
		this.height = 0;
	}


	public bool IsInsideBounds (int x, int y) {
		if (x < 1 || y < 1 || x > width - 2 || y > height - 2) {
			return false;
		}

		return true;
	}


	// Tile

	public Tile CreateTile (System.Type TileType, int x, int y, float scale = 1, Sprite asset = null) {
		Transform parent = container.Find("Tiles");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = TileType.ToString(); //typeof(T).ToString();

		Tile tile = obj.AddComponent(TileType) as Tile;
		tile.Init(this, x, y, scale, asset);

		SetTile(x, y, tile);

		return tile;
	}


	public void SetTile (int x, int y, Tile tile) {
		// refresh astar walkability
		Astar.instance.walkability[x, y] = (tile != null && !tile.IsWalkable()) ? 1 : 0;

		layers.Set<Tile>(y, x, tile);
	}


	public Tile GetTile (int x, int y) {
		return layers.Get<Tile>(y, x);
	}

	public Tile GetTile (Vector3 pos) {
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);

		return layers.Get<Tile>(y, x);
	}


	public bool TileIsOpaque (int x, int y) {
		if (!IsInsideBounds(x, y)) { return true; }

		Tile tile = GetTile(x, y);
		if (tile == null) { return true; }

		return tile.IsOpaque();
	}


	// Entity

	public Entity CreateEntity (System.Type EntityType, int x, int y, float scale = 1, Sprite asset = null) {
		Transform parent = container.Find("Entities");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = EntityType.ToString(); //typeof(T).ToString();

		Entity entity = obj.AddComponent(EntityType) as Entity;
		entity.Init(this, x, y, scale, asset);

		SetEntity(x, y, entity);

		return entity;
	}


	public void SetEntity (int x, int y, Entity entity) {
		// refresh astar walkability
		Astar.instance.walkability[x, y] = (entity != null && !entity.IsWalkable()) ? 1 : 0;

		layers.Set<Entity>(y, x, entity);
	}


	public Entity GetEntity (int x, int y) {
		return layers.Get<Entity>(y, x);
	}


	// Creature

	public Creature CreateCreature (System.Type CreatureType, int x, int y, float scale = 1, Sprite asset = null) {
		Transform parent = container.Find("Creatures");

		GameObject obj = (GameObject)Instantiate(tilePrefab);
		obj.transform.SetParent(parent, false);
		obj.name = CreatureType.ToString(); //typeof(T).ToString();

		Creature creature = obj.AddComponent(CreatureType) as Creature;
		creature.Init(this, x, y, scale,  asset);

		SetCreature(x, y, creature);

		return creature;
	}


	public void SetCreature (int x, int y, Creature creature) {
		// refresh astar walkability
		Astar.instance.walkability[x, y] = (creature != null && !creature.IsWalkable()) ? 1 : 0;

		layers.Set<Creature>(y, x, creature);
	}


	public Creature GetCreature (int x, int y) {
		return layers.Get<Creature>(y, x);
	}
}
