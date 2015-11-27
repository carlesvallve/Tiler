using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

	public static GameObject target;
	public float zoomSpeed = 1f;
	public float dragSpeed = 1f;

	private Vector3 lastMousePos;


	void LateUpdate () {
		ZoomIntoPosition(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Input.mousePosition);

		if (Input.GetMouseButtonDown(1)) {
			lastMousePos = Input.mousePosition;
		}

		if (Input.GetMouseButton(1)) {
			Vector2 delta = (Input.mousePosition - lastMousePos) * dragSpeed;
			Drag(delta);
			lastMousePos = Input.mousePosition;
		}	
	}


	private void ZoomIntoPosition (float delta, Vector2 position) {
		Vector3 preZoomWorldPosition = Camera.main.ScreenToWorldPoint(position);
		PureZoom(delta);
		Vector2 postZoomScreenPosition = Camera.main.WorldToScreenPoint(preZoomWorldPosition);
		Drag(position- postZoomScreenPosition);
	}


	void PureZoom (float delta) {
		float min = 1;
		float max = 1000;
		float newSize = Mathf.Clamp (Camera.main.orthographicSize - delta, min, max);
		Camera.main.orthographicSize = newSize;
		//Camera.main.transform.localScale = new Vector3 (newSize, newSize, 1);

		// then constrain to bounds if needed
	}
 
	void Drag (Vector2 delta) {
		float pixelSizeAdjustment = 1f;
		delta *= pixelSizeAdjustment;
		Vector2 offset = Vector2.Scale(delta, Camera.main.transform.localScale);
		Camera.main.transform.localPosition -= (Vector3)offset;

		// then constrain to bounds if needed
	}

}
