using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Controls : MonoBehaviour {

	private Grid grid;
	

	void Awake () {
		grid = Grid.instance;
	}
	
	void Update () {
		// escape if mouse is over hud
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		if (Input.GetMouseButtonDown(0)) {
			TapAtPos(Input.mousePosition);
		}

		if (Input.GetMouseButtonDown(1)) {
			InfoAtPos(Input.mousePosition);
		}
	}


	private void TapAtPos (Vector3 pos) {
		// get tap position in world/grid units
		pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

		// get goal coordinates
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);

		// escape if coordinates are out of grid bounds
		if (pos.x < 0 || pos.y < 0 || pos.x > grid.width - 1 || pos.y > grid.height - 1) {
			return;
		}

		// try to move player to goal
		/*if (!grid.player.moving) {
			grid.player.SetPath(x, y);
		} else {
			grid.player.StopMoving();
		}*/

		grid.player.SetPath(x, y);
		
	}


	private void InfoAtPos (Vector3 pos) {
		// get tap position in world/grid units
		pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

		// get goal coordinates
		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(pos.y);

		

		Creature creature = grid.GetCreature(x, y);
		if (creature != null) {
			Hud.instance.Log("You see " + GetTileName(creature));
			return;
		}

		Entity entity = grid.GetEntity(x, y);
		if (entity != null) {
			//string adj = entity.state != EntityStates.None ? entity.state.ToString() : "";
			Hud.instance.Log("You see " + GetTileName(entity)); // adj + " " + 
			return;
		}

		Tile tile = grid.GetTile(x, y);
		if (tile != null) {
			Hud.instance.Log("You see " + GetTileName(tile));
			return;
		}
	}


	private string GetTileName (Tile tile) {
		string[] arr = tile.asset.name.Split('-');
		string desc = arr[0];

		if (arr.Length > 1) {
			int n = 0;
			if (arr[1].Length > 1 && !System.Int32.TryParse(arr[1], out n)) { 
				desc = arr[1] + " " + arr[0]; 
			}
		}

		string last = desc.Substring(desc.Length -1, 1);
		if (last == "s" || last == "gold") {
			desc = "some " + desc;
		} else {
			string first = desc.Substring(0, 1);
			if (first == "a" || first == "e" || first == "i" || first == "o" || first == "u") {
				desc = "an " + desc;
			} else {
				desc = "a " + desc;
			}
		}

		return desc;
	}
}
