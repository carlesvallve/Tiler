using UnityEngine;
using System.Collections;


public class Door : Entity {


	public override void Init (Grid grid, int x, int y, float scale = 1, Sprite asset = null) {
		base.Init(grid, x, y, scale, asset);
		walkable = true;

		SetImages(scale, Vector3.zero, 0.04f);

		state = EntityStates.Closed;
	}


	public override void SetState (EntityStates state) {
		this.state = state;
		SetAsset(Game.assets.dungeon[state == EntityStates.Open ? "door-open" : "door-closed"]);
	}


	public IEnumerator Open () {
		state = EntityStates.Open;
		sfx.Play("Audio/Sfx/Door/key", 1f, Random.Range(0.4f, 0.6f));

		SetAsset(Game.assets.dungeon["door-open"]);
		
		yield return new WaitForSeconds(0.5f);
	}


	public IEnumerator Unlock (System.Action<bool> cb) {
		sfx.Play("Audio/Sfx/Door/unlock", 0.8f, Random.Range(0.8f, 1.2f));

		bool success = Random.Range(1, 100) < 50;
		if (!success) {
			Speak("Locked", Color.white);
		}

		yield return new WaitForSeconds(0.5f);

		if (success) {
			Speak("Success!", Color.white);
			sfx.Play("Audio/Sfx/Door/door-open2", 0.8f, Random.Range(0.8f, 1.2f));
			state = EntityStates.Closed;
		}

		cb(success);
		yield break;
	}
	
}
