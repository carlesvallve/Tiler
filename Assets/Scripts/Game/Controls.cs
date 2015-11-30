using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	private Grid grid;
	

	void Awake () {
		grid = Grid.instance;
	}
	
	void Update () {
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

		// try to move player to goal
		grid.player.SetPath(x, y);
	}
}
