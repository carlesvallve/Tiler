using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Entity: Tile {

	public EntityStates state { get; set; }

	protected List<Item> items = new List<Item>();

	public bool breakable = false;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = false;

		SetSortingOrder(100);
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

}
