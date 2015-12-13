using UnityEngine;
using System.Collections;


public class Item : Entity {

	public int ammount = 1;
	public bool equippable = false;


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);
	}


	protected virtual string GetRandomAssetName () {
		return null;
	}


	// Pickup

	public virtual void Pickup (Creature creature) {
		// spawn glow particles
		if (creature.visible) {
			Grid.instance.CreateGlow(transform.position, 8, Color.white);
		}

		// add to creature's inventory
		creature.inventory.AddItem(this);

		// reparent item to creature
		transform.SetParent(creature.transform, false);
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);

		grid.SetEntity(x, y, null);
	}


	// Drop

	public virtual void Drop (Tile tile, int x, int y) {
		transform.SetParent(grid.container.Find("Entities"), false);
		transform.localPosition = new Vector3(tile.x, tile.y, 0);
		gameObject.SetActive(true);

		sfx.Play("Audio/Sfx/Item/armour", 0.6f, Random.Range(0.8f, 1.2f));

		// Animate items interpolating them form chest position to x,y
		StartCoroutine(DropAnimation(x, y, 0.1f));

		UpdateVisibility();

		// remove from creature's inventory dictionary
		if (tile is Creature) {
			Creature creature = (Creature)tile;
			creature.inventory.RemoveItem(this);
		}
	}


	private IEnumerator DropAnimation (int x, int y, float duration = 0.1f) {
		// interpolate item position
		float t = 0;
		Vector3 startPos = transform.localPosition;
		Vector3 endPos = new Vector3(x, y, 0);
		while (t <= 1) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		// set item in grid array
		LocateAtCoords(x, y);

		UpdateVisibility();
	}


	// Use

	public virtual void Use (Creature creature) {}
	public virtual bool CanUse (Creature creature) { return false; }

}
