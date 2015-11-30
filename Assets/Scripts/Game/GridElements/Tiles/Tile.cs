using UnityEngine;
using System.Collections;


public class Tile : MonoBehaviour {

	protected Grid grid;
	
	public int x { get; set; }
	public int y { get; set; }
	public bool walkable { get; set; }
	public bool visited { get; set; }

	protected Sprite asset;
	protected SpriteRenderer outline;
	protected SpriteRenderer img;
	

	public virtual void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		outline = transform.Find("Outline").GetComponent<SpriteRenderer>();
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();

		this.grid = grid;
		this.x = x;
		this.y = y;
		this.asset = asset;

		this.walkable = true;

		transform.localPosition = new Vector3(x, y, 0);

		SetSortingOrder(0);
		SetImage(scale, Vector3.zero, 0);
	}


	protected void SetSortingOrder (int zIndex) {
		zIndex += grid.height - this.y;
		outline.sortingOrder = zIndex;
		img.sortingOrder = zIndex + 1;
	}


	protected void SetImage (float scale, Vector3 pos, float outlineDistance = 0f) {
		outline.transform.localPosition = pos + new Vector3(outlineDistance, -outlineDistance, 0);
		outline.transform.localScale = new Vector3(scale, scale, 1);
		outline.sprite = asset;
		outline.gameObject.SetActive(outlineDistance != 0);
		
		img.transform.localPosition = pos + new Vector3(-outlineDistance, outlineDistance, 0);
		img.transform.localScale = new Vector3(scale, scale, 1);
		img.sprite = asset; 
	}


	public void SetColor (Color color) {
		img.color = color; 
	}


	public bool IsWalkable () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && !entity.walkable) { return false; }

		Creature creature = grid.GetCreature(x, y);
		if (creature != null && !creature.walkable) { return false; }

		return walkable;
	}


	public bool IsOccupied () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) { return true; }

		Creature creature = grid.GetCreature(x, y);
		if (creature != null) { return true; }

		return false;
	}
	
}
