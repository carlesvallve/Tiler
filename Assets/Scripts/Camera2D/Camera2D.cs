using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class Camera2D : MonoBehaviour {

	public static GameObject target;
	public float zoomSpeed = 1f;
	public float dragSpeed = 1f;

	private Vector3 lastMousePos;


	void LateUpdate () {
		// escape if mouse is over hud
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		// set zoom
		//Vector3 screenPos = camera.WorldToScreenPoint(target.position);
		//Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		ZoomIntoPosition(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Input.mousePosition);

		// set drag
		if (Input.GetMouseButtonDown(2)) {
			lastMousePos = Input.mousePosition;
		} else if (Input.GetMouseButton(2)) {
			Vector2 delta = (Input.mousePosition - lastMousePos) * dragSpeed;
			Drag(delta);
			lastMousePos = Input.mousePosition;
		}	
	}


	private void ZoomIntoPosition (float delta, Vector2 position) {
		//Vector3 preZoomWorldPosition = Camera.main.ScreenToWorldPoint(position);
		PureZoom(delta);
		//Vector2 postZoomScreenPosition = Camera.main.WorldToScreenPoint(preZoomWorldPosition);
		//Drag(position- postZoomScreenPosition);
		Drag(Vector2.zero);
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
