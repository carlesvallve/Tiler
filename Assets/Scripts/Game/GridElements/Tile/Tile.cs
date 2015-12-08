﻿using UnityEngine;
using System.Collections;


public class Tile : MonoBehaviour {

	protected bool debugEnabled = false;

	protected Grid grid;
	protected AudioManager sfx;
	
	public int x { get; set; }
	public int y { get; set; }
	public int roomId { get; set; }

	public int fovTurn;
	public float fovDistance;

	public bool walkable { get; set; }
	public bool visible { get; set; } // seen by the player right now
	//public bool shadowed { get; set; }
	public bool explored { get; set; } // discovered by the player

	public Color color;

	public Sprite asset { get; private set; }

	protected Transform container;
	protected SpriteRenderer shadow;
	protected SpriteRenderer outline;
	protected SpriteRenderer img;
	protected TextMesh label;

	protected int zIndex;


	public virtual void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		sfx = AudioManager.instance;

		container = transform.Find("Sprites");
		outline = transform.Find("Sprites/Outline").GetComponent<SpriteRenderer>();
		img = transform.Find("Sprites/Sprite").GetComponent<SpriteRenderer>();
		shadow = transform.Find("Sprites/Shadow").GetComponent<SpriteRenderer>();
		shadow.gameObject.SetActive(false);
		
		label = transform.Find("Label").GetComponent<TextMesh>();
		label.GetComponent<Renderer>().sortingLayerName = "Ui";
		label.gameObject.SetActive(debugEnabled);

		this.grid = grid;
		this.x = x;
		this.y = y;
		this.asset = asset;

		this.walkable = true;

		transform.localPosition = new Vector3(x, y, 0);

		SetAsset(asset);
		SetImages(scale, Vector3.zero, 0);
		SetSortingOrder(0);

		visible = false;
		explored = false;
	}


	public virtual void LocateAtCoords (int x, int y) {
		UpdatePosInGrid(x, y);
		transform.localPosition = new Vector3(x, y, 0);
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
		if (!debugEnabled) { 
			return; 
		}

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


	protected virtual void SetSortingOrder (int zIndex) {
		this.zIndex = zIndex;

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


	public virtual void SetFovInfo (int turn, float distance) {
		// generate tile fov info, used by monster ai for chase and follow behaviour
		if (IsPassable()) {
			fovTurn = turn;
			fovDistance = distance;

			// debug fov info
			SetInfo(fovTurn.ToString() + "\n" + fovDistance.ToString(), Color.white);
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
	public bool IsWalkable () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && !entity.walkable) { return false; }

		Creature creature = grid.GetCreature(x, y);
		if (creature != null && !creature.walkable) { return false; }

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
	
}