using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Entity: Tile {

	public EntityStates state { get; set; }

	public List<Item> items = new List<Item>();
	public bool breakable = false;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null, string id = null) {
		zIndex = 100;

		base.Init(grid, x, y, scale, asset);
		walkable = false;

		//SetSortingOrder(100);
		SetImages(scale, new Vector3(0, 0.1f, 0), 0.04f);
	}


	public virtual void SetState (EntityStates state) {
		this.state = state;
	}


	protected override void UpdatePosInGrid (int x, int y) {
		grid.SetEntity(this.x, this.y, null);
		this.x = x;
		this.y = y;
		grid.SetEntity(x, y, this);
	}


	public IEnumerator Break (Color color, float delay = 0) {
		yield return new WaitForSeconds(delay);

		sfx.Play("Audio/Sfx/Combat/hitB", 0.6f, Random.Range(0.5f, 2.0f));

		grid.CreateBlood(transform.localPosition, 16, color);

		Entity entity = grid.GetEntity(x, y);
		if (entity == null || entity == this) {
			grid.SetEntity(this.x, this.y, null);
		}
		
		Destroy(gameObject);
	}


	/*public override void SetVisibility (Tile tile, bool visible, float shadowValue) {
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
	}*/

}
