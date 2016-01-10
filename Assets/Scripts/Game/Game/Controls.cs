using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class Controls : MonoBehaviour {

	private Grid grid;

	private bool longPress = false;
	private float longPressDuration = 0.25f;
	private float tapStartTime = 0;
	//private float timeDelta = 0;


	void Awake () {
		grid = Grid.instance;
	}

	
	void Update () {
		// Escape if mouse is over any UI
		if (EventSystem.current.IsPointerOverGameObject()  || 
			EventSystem.current.IsPointerOverGameObject(0) || 
			EventSystem.current.IsPointerOverGameObject(-1)
		) { return; }

		// Escape if any popup is active
		if (Hud.instance.IsPopupOpen()) {
			return;
		}

		// tap start
		if (Input.GetMouseButtonDown(0)) {
			tapStartTime = Time.time;
		}

		// tapping
		if (Input.GetMouseButton(0)) {
			float timeDelta = Time.time - tapStartTime;
			if (longPress == false && timeDelta >= longPressDuration) {
				SetInfoAtPos(Input.mousePosition);
				longPress = true;
			}
		}

		// tap release
		if (Input.GetMouseButtonUp(0)) {
			if (!longPress) {
				TapAtPos(Input.mousePosition);
			}
			longPress = false;
		}
	}


	private void TapAtPos (Vector3 pos) {
		if (!CanTap()) {
			if (grid.player.state == CreatureStates.Moving) {
				grid.player.markedToStop = true;
			}
			return;
		}

		// get tap position in world/grid units
		pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

		// get goal coordinates
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);

		// escape if coordinates are out of grid bounds
		if (pos.x < 0 || pos.y < 0 || pos.x > grid.width - 1 || pos.y > grid.height - 1) {
			return;
		}

		// tell player to look for an astar path and follow it
		grid.player.markedToStop = false;
		grid.player.SetPath(x, y);
	}


	private bool CanTap () {
		// make sure player is in idle state
		if (grid.player.state != CreatureStates.Idle) {
			return false;
		}

		// make sure that all monsters are in idle state
		Monster[] monsters = FindObjectsOfType<Monster>();
		int c = 0;
		foreach (Monster monster in monsters) {
			if (monster.state == CreatureStates.Idle) {
				c++;
			}
		}

		//print ("Idle monsters " + c + "/" + monsters.Length);
		if (c == monsters.Length) {
			return true;
		}

		return false;
	}


	private void SetInfoAtPos (Vector3 pos) {
		// get tap position in world/grid units
		pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

		// get goal coordinates
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);

		// center camera on tile
		Tile tile = grid.GetTile(x, y);
		if (tile != null) {
			Camera2D.instance.CenterCamera(tile, true);
		}

		Creature creature = grid.GetCreature(x, y);
		if (creature != null) {
			Hud.instance.Log("You see " + 
				Descriptions.GetTileDescription(creature) + " " + 
				Descriptions.GetEquipmentDescription(creature)
			);
			return;
		}

		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {
			Hud.instance.Log("You see " + Descriptions.GetTileDescription(entity));
			return;
		}

		//Tile tile = grid.GetTile(x, y);
		if (tile != null) {
			Hud.instance.Log("You see " + Descriptions.GetTileDescription(tile));
			return;
		}
	}

}
