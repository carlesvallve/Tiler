using UnityEngine;
using System.Collections;


public class Item : Entity {

	public string typeId;
	public string soundId;
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


	public virtual void Pickup () {
		Grid.instance.CreateGlow(transform.position, 5);
		Destroy(gameObject);
	}
}
