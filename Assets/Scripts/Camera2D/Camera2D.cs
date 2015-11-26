using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	public float zoomSpeed = 1f;
	public float panSpeed = 1f;

	public static GameObject target;

	private bool isMouseDown;
	private Vector3 lastMousePos;
	private Vector2 delta;

	public bool panning { get; private set; }


	void LateUpdate () {
		Zoom();
		//Pan();
		Track();	
	}


	private void Zoom () {
		float zoomDelta = -Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize += zoomDelta * zoomSpeed;
	}


	private void Pan () {
		if (Input.GetMouseButtonDown(0)) {
			lastMousePos = Input.mousePosition;
			isMouseDown = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
			panning = false;
		}

		if (isMouseDown) {
			Vector3 vec = Input.mousePosition - lastMousePos;
			lastMousePos = Input.mousePosition;

			if (vec.magnitude >= 1) {
				panning = true;
				delta -= new Vector2(vec.x, vec.y) / (Camera.main.orthographicSize * 1);
			}
		}
	}


	private void Track () {
		if (target != null) {
			Vector3 pos = new Vector3(
				target.transform.position.x + delta.x, 
				target.transform.position.y + delta.y, 
				-10
			);

			Camera.main.transform.position = pos;
			//Camera.main.transform.position = Vector3.Lerp(transform.position, pos, Time.time * 0.01f);
		}
	}
}
