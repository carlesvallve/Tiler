using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour {

	protected Grid grid;
	protected AudioManager sfx;
	
	public int x { get; set; }
	public int y { get; set; }
	public int roomId { get; set; }

	public bool walkable { get; set; }
	public bool visible { get; set; }
	public bool explored { get; set; }

	public Sprite asset { get; set; }
	public Color color;

	protected int zIndex = 0;

	protected Transform container;
	protected SpriteRenderer shadow;
	protected SpriteRenderer outline;
	public SpriteRenderer img { get; private set; }
	protected TextMesh label;
	
	// used by ai
	public int interestWeight = 0;
	public int fovTurn;
	public float fovDistance;


	public virtual void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		sfx = AudioManager.instance;

		container = transform.Find("Sprites");
		outline = transform.Find("Sprites/Outline").GetComponent<SpriteRenderer>();
		img = transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>();
		shadow = transform.Find("Sprites/Shadow").GetComponent<SpriteRenderer>();
		shadow.gameObject.SetActive(false);
		
		label = transform.Find("Label").GetComponent<TextMesh>();
		label.GetComponent<Renderer>().sortingLayerName = "Ui";
		label.gameObject.SetActive(Debug.isDebugBuild);

		this.grid = grid;
		this.x = x;
		this.y = y;
		this.asset = asset;

		this.walkable = true;

		transform.localPosition = new Vector3(x, y, 0);

		SetAsset(asset);
		SetImages(scale, Vector3.zero, 0);
		SetSortingOrder();

		visible = false;
		explored = false;
	}


	public virtual void LocateAtCoords (int x, int y) {
		UpdatePosInGrid(x, y);
		transform.localPosition = new Vector3(x, y, 0);
		SetSortingOrder();
	}
	

	protected virtual void UpdatePosInGrid (int x, int y) {
		grid.SetTile(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetTile(x, y, this);
	}


	// =====================================================
	// Set tile elements
	// =====================================================

	public virtual void Speak (string str, Color color, bool stick = false) {
		Hud.instance.CreateLabel(this, str, color, stick);
	}


	public void SetInfo (string str, Color color) {
		label.color = color;
		label.text = str;
		label.gameObject.SetActive(str != null && str != "");
	}


	protected void SetAsset (Sprite asset) {
		this.asset = asset;
		
		outline.sprite = asset;
		img.sprite = asset;
		shadow.sprite = asset;
	}


	protected void SetImages (float scale, Vector3 pos, float outlineDistance = 0f) {
		outline.transform.localPosition = pos + new Vector3(outlineDistance, -outlineDistance, 0);
		outline.transform.localScale = new Vector3(scale, scale, 1);
		outline.gameObject.SetActive(outlineDistance != 0);
		
		img.transform.localPosition = pos + new Vector3(-outlineDistance, outlineDistance, 0);
		img.transform.localScale = new Vector3(scale, scale, 1); 

		shadow.transform.localPosition = pos + new Vector3(-outlineDistance, outlineDistance, 0);
		shadow.transform.localScale = new Vector3(scale, scale, 1);
	}


	protected virtual void SetSortingOrder () {
		//this.zIndex = zIndex;

		zIndex += grid.height - this.y;
		outline.sortingOrder = zIndex;
		img.sortingOrder = zIndex + 1;
		shadow.sortingOrder = zIndex + 2;
		//label.offsetZ = -1; //zIndex + 2;
	}


	public void SetColor (Color color, bool assignColor = false) {
		if (assignColor) { this.color = color; }
		img.color = color;
	}


	// =====================================================
	// Visibility
	// =====================================================

	public virtual void SetVisibility (Tile tile, bool visible, float shadowValue) {
		// note: we need to pass the tile because we want 
		// to use the original tile's 'explored' flag for entities and creatures

		// seen by the player right now
		this.visible = visible; 
		container.gameObject.SetActive(visible || tile.explored);

		// apply shadow
		SetShadow(visible ? shadowValue : 1);
		if (!visible && tile.explored) { SetShadow(0.6f); }

		// once we have seen the tile, mark the tile as explored
		if (visible) {
			tile.explored = true;
		}
	}


	public virtual void UpdateVisibility () {
		// creatures need to set their visibility also after they moved
		Tile tile = grid.GetTile(x, y);
		if (tile != null) {
			SetVisibility(tile, tile.visible, tile.GetShadowValue());
		}
	}


	public virtual void SetFovInfo (int turn, float distance, bool debug = false) {
		// generate tile fov info, used by monster ai for chase and follow behaviour
		if (IsPassable()) {
			fovTurn = turn;
			fovDistance = distance;

			// debug fov info
			if (debug) {
				SetInfo(fovTurn.ToString() + "\n" + fovDistance.ToString(), Color.white);
			}
		} 
	}


	public void SetShadow (float value) {
		shadow.color = new Color(0, 0, 0, value);
		shadow.gameObject.SetActive(value > 0);
	}


	public float GetShadowValue () {
		return shadow.color.a;
	}

		
	// =====================================================
	// Get tile states
	// =====================================================

	// only computes walkable entities
	public bool IsPassable () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && !entity.walkable) { return false; }

		return walkable;
	}

	// computes walkable entities and creatures
	public bool IsWalkable () { // Creature who = null
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && !entity.walkable) { return false; }

		Creature creature = grid.GetCreature(x, y);
		if (creature != null && !creature.walkable) { return false; }

		/*Creature creature = grid.GetCreature(x, y);
		if (creature != null) {
			if (who == null) {
				if (!creature.walkable) { return false; }
			} else {
				if (who.IsAgressive()) { return true; }
			}
			
		}*/

		return walkable;
	}


	// computes all entities that block the light
	public bool IsOpaque () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {
			if (entity is Wall) { return true; }
			if (entity is Door && ((Door)entity).state != EntityStates.Open) { return true; }
		}
		
		return false;
	}

	// computes all entities and creatures, even if they are walkable
	public bool IsOccupied () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) { return true; }

		Creature creature = grid.GetCreature(x, y);
		if (creature != null) { return true; }

		return false;
	}


	// =====================================================
	// Methods shared by both entities and creatures
	// =====================================================

	public virtual void SpawnItemsFromInventory (List<Item> allItems, bool useCenterTile = true) {
		if (allItems.Count == 0) { return; }

		// get neighbours, not including the creature's tile, which will be manually added for the first item
		List<Tile> neighbours = grid.GetNonOccupiedNeighbours(this.x, this.y, false);

		//randomize item list
		Utils.Shuffle(allItems);

		Item item = null;

		// spawn first item always on creature's tile (except if occupied by a an entity)
		// the goal here is not to spawn an entity where another one already exists (i,e: over stairs)
		if (useCenterTile) {
			Tile tile = grid.GetTile(x, y);
			if (tile != null && !tile.IsOccupied()) {
				item = allItems[0];
				item.Drop(this, this.x, this.y);
				allItems.RemoveAt(0);
			}
		}
		
		// spawn one item for each available neighbour
		foreach (Tile tile in neighbours) {
			if (allItems.Count == 0) { return; }

			item = allItems[0];
			item.Drop(this, tile.x, tile.y);
			allItems.RemoveAt(0);
		}
	}
	

}
