using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class Grid : MonoBehaviour {

	// grid props
	public GridLayers layers;

	public int width = 7;
	public int height = 9;
	public int tileWidth = 30;
	public int tileHeight = 25;

	public Player player { get; set; }
	public Ladder ladderUp { get; set; }
	public Ladder ladderDown { get; set; }

	// interaction
	private Vector3 swipeStart;
	private List<Vector2> deltas = new List<Vector2>();

	// inspector
	public Transform container;
	public GridPrefabs prefabs;


	// ===============================================================
	// Grid Initialization
	// ===============================================================

	public void Init () {
		prefabs.Init();
		InitializeGrid();
	}


	public void InitializeGrid (int width = 7, int height = 11) {
		this.width = width;
		this.height = height;

		// destroy all previously generated gameobjects
		ResetGrid();

		// initialize grid layers
		layers = new GridLayers () {
			{ typeof(Tile), new GridLayer<Tile> (height, width) },
			{ typeof(Entity), new GridLayer<Entity> (height, width) },
		};

		// initialize grid tiles
		/*for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				Tile tile = CreateTile(TileTypes.Empty, x, y);
				SetTile(x, y, tile);
			}
		}*/
	}


	private void ResetGrid () {
		Tile[] tilesToDestroy = container.GetComponentsInChildren<Tile>();
		foreach (Tile tile in tilesToDestroy) {
			Destroy(tile.gameObject);
		}

		Entity[] entitiesToDestroy = container.GetComponentsInChildren<Entity>();
		foreach (Entity entity in entitiesToDestroy) {
			Destroy(entity.gameObject);
		}
	}


	public void BatchGrid () {
		GameObject go = container.Find("Tiles").gameObject;
		go.SetActive( false );
     	go.SetActive( true );
	}


	// ===============================================================
	// Tiles
	// ===============================================================

	public Tile CreateTile (int x, int y, TileTypes type, Color color) {
		Transform parent = container.Find("Tiles/" + type.ToString());

		GameObject obj = (GameObject)Instantiate(prefabs.tiles[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString();

		Tile tile = obj.GetComponent<Tile>();
		tile.Init(this, type, x, y, color);

		SetTile(x, y, tile);

		return tile;
	}


	public void ChangeTile (Tile tile, TileTypes type, Color color) {
		SetTile(tile.x, tile.y, CreateTile(tile.x, tile.y, type, color));
		Destroy(tile.gameObject);
	}


	public void SetTile (int x, int y, Tile tile) {
		layers.Set<Tile>(y, x, tile);
	}

	
	public void SetEntity (int x, int y, Entity entity) {
		layers.Set<Entity>(y, x, entity);
	}


	public Tile GetTile (int x, int y) {
		return layers.Get<Tile>(y, x);
	}


	public Tile GetTile (Vector3 pos) {
		float ratio = tileHeight / (float)tileWidth;

		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(- 0.4f + pos.y / ratio);

		return layers.Get<Tile>(y, x);
	}


	public Entity GetEntity (int x, int y) {
		return layers.Get<Entity>(y, x);
	}


	public Entity GetEntity (Vector3 pos) {
		float ratio = tileHeight / (float)tileWidth;

		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(- 0.4f + pos.y / ratio);

		return layers.Get<Entity>(y, x);
	}


	// ===============================================================
	// Entities
	// ===============================================================

	public Obstacle CreateObstacle (int x, int y, ObstacleTypes type, Color color) {
		Transform parent = this.container.Find("Entities/Obstacles");

		GameObject obj = GameObject.Instantiate(prefabs.obstacles[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString(); //"Obstacle";
		Obstacle obstacle = obj.GetComponent<Obstacle>();
		obstacle.Init(this, x, y, color);

		obstacle.type = type;

		return obstacle;
	}


	public Item CreateItem (int x, int y, ItemTypes type) {
		Transform parent = this.container.Find("Entities/Items");

		GameObject obj = GameObject.Instantiate(prefabs.items[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString();
		Item item = obj.GetComponent<Item>();
		item.Init(this, x, y, Color.white);

		item.type = type;
		
		return item;
	}


	public Door CreateDoor (int x, int y, DoorTypes type, DoorStates state, DoorDirections direction) {
		Transform parent = this.container.Find("Entities/Doors");

		GameObject obj = GameObject.Instantiate(prefabs.doors[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString();
		Door door = obj.GetComponent<Door>();
		door.Init(this, x, y, Color.white);

		door.type = type;
		door.state = state;
		door.SetDirection(direction);

		return door;
	}


	public Ladder CreateLadder (int x, int y, LadderTypes type, LadderDirections direction) {
		Transform parent = this.container.Find("Entities/Ladders");

		GameObject obj = GameObject.Instantiate(prefabs.ladders[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString();
		Ladder ladder = obj.GetComponent<Ladder>();
		ladder.Init(this, x, y, Color.white);

		ladder.type = type;
		ladder.SetDirection(direction);
		
		return ladder;
	}


	public Player CreatePlayer (int x, int y, PlayerTypes type) {
		Transform parent = this.container.Find("Entities/Players");

		GameObject obj = GameObject.Instantiate(prefabs.players[type]);
		obj.transform.SetParent(parent, false);
		obj.name = type.ToString();
		Player player = obj.GetComponent<Player>();
		player.Init(this, x, y, Color.white);

		player.type = type;

		return player;
	}


	// ===============================================================
	// Interaction
	// ===============================================================

	private bool isMouseDown = false;


	void Update () {
		// escape if mouse is over any UI
		if (EventSystem.current != null && 
			(EventSystem.current.IsPointerOverGameObject(0) || 
			EventSystem.current.IsPointerOverGameObject(-1))) {
			return;
		}

		OnMouseInteraction ();
	}


	private void OnMouseInteraction () {
		
		if (Input.GetMouseButtonDown(0)) {
			swipeStart = Input.mousePosition;
			isMouseDown = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
		}

		if (isMouseDown) {
			Vector3 vec = Input.mousePosition - swipeStart;
			
			if (vec.magnitude > 16) {
				Vector2 delta = vec.normalized; 
				if (delta.x >= 0.5f) { delta.x = 1; } else if (delta.x <= -0.5f) { delta.x = -1f; } else { delta.x = 0; }
				if (delta.y >= 0.5f) { delta.y = 1; } else if (delta.y <= -0.5f) { delta.y = -1f; } else { delta.y = 0; }

				if (player.moving) {
					deltas.Add(delta);
				} else {
					Swipe(delta);
				}
				
				isMouseDown = false;
			}
		}
	}


	public void NextSwipe () {
		if (deltas.Count > 0) {
			Swipe(deltas[0]);
			deltas.RemoveAt(0);
		}
	}


	private void Swipe (Vector2 delta) {
		int dx = (int)delta.x;
		int dy = (int)delta.y;
		int x = player.x;
		int y = player.y;

		x += dx;
		y += dy;

		player.MoveToCoords(x, y, 0.15f);
	}
}
