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
		grid.player.SetPath(x, y);
	}
}
