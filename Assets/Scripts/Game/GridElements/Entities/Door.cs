using UnityEngine;
using System.Collections;


public class Door : Entity {

	public EntityStates state { get; set; }


	public override void Init (Grid grid, int x, int y, Sprite asset, float scale = 1) {
		base.Init(grid, x, y, asset, scale);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		state = EntityStates.Closed;
	}


	public IEnumerator Open () {
		SetAsset(Game.assets.dungeon["door-open"]);
		state = EntityStates.Open;
		sfx.Play("Audio/Sfx/Door/key", 1f, Random.Range(0.4f, 0.6f));

		yield return new WaitForSeconds(0.5f);
	}


	public IEnumerator Unlock (System.Action<bool> cb) {
		sfx.Play("Audio/Sfx/Door/unlock", 0.8f, Random.Range(0.8f, 1.2f));
		yield return new WaitForSeconds(0.5f);

		bool success = Random.Range(1, 100) < 50;
		if (success) {
			yield return StartCoroutine(Open());
		} 

		cb(success);
		yield break;
	}
	
}
