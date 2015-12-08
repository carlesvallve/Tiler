using UnityEngine;
using System.Collections;


public class Item : Entity {

	public string typeId;
	public int ammount;

	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		//print (typeId + " " + asset);
	}


	protected virtual string GetRandomAssetName () {
		return null;
	}


	public virtual void Pickup (Creature creature) {
		// spawn glow particles
		if (creature.visible) {
			Grid.instance.CreateGlow(transform.position, 8);
		}

		// add to creature's items dictionary
		creature.items[typeId].Add(this);

		// destroy item in grid
		//Destroy(gameObject); // -> The item in the dictionary will be destroyed too!

		transform.SetParent(creature.transform, false);
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);

		grid.SetEntity(x, y, null);
	}


	public virtual void Drop (Tile tile, int x, int y) {
		transform.SetParent(grid.container.Find("Entities"), false);
		LocateAtCoords(x, y);
		gameObject.SetActive(true);

		sfx.Play("Audio/Sfx/Item/armour", 0.6f, Random.Range(0.8f, 1.2f));

		// TODO: Animate items interpolating them form creature pos to x,y
	}
}
