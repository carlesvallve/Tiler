﻿using UnityEngine;
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


	public IEnumerator Open (Creature creature, bool hasBeenUnlocked = false) {
		sfx.Play("Audio/Sfx/Door/key", 1f, Random.Range(0.4f, 0.6f));
		state = EntityStates.Open;

		SetAsset(Game.assets.dungeon["door-open"]);

		if (creature is Player) { 
			if (!hasBeenUnlocked) { 
				Hud.instance.Log("You open the door."); 
			}
			creature.MoveCameraTo(this.x, this.y);
		}
		
		yield break;
	}


	public IEnumerator Unlock (Creature creature) { // , System.Action<bool> cb // cb(success);
		bool success = Random.Range(1, 100) < 50;

		if (success) {
			state = EntityStates.Closed;
			sfx.Play("Audio/Sfx/Door/door-open2", 0.8f, Random.Range(0.8f, 1.2f));
			if (creature is Player) { 
				Speak("Success!", Color.white);
				Hud.instance.Log("You unlock the door."); 
			}

		} else {
			sfx.Play("Audio/Sfx/Door/unlock", 0.8f, Random.Range(0.8f, 1.2f));
			if (creature is Player) { 
				Speak("Locked", Color.white);
				Hud.instance.Log("The door is locked.");
			}
		}

		yield return new WaitForSeconds(0.25f);

		if (success) {
			StartCoroutine(Open(creature, true));
		}
	}
	
}
