using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class Camera2D : MonoBehaviour {

	public static Camera2D instance;

	public static Transform target;
	public float zoomSpeed = 1f;
	public float dragSpeed = 1f;
	public bool autoZoom = false;

	private Vector3 lastMousePos;

	void Awake() {
		instance = this;

		Camera.main.orthographicSize = Screen.height / 64;
	}

	void LateUpdate () {
		// set fixed zoom
		if (autoZoom) {
			Camera.main.orthographicSize = Screen.height / 64;
		}
		
		// escape if mouse is over hud
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		// set zoom
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


	public IEnumerator MoveToPos (Vector2 pos) {
		float duration = 0.5f;
		float t = 0;
		Vector3 endPos = new Vector3(pos.x, pos.y, -10);

		while (t <= 1) {
			t += Time.deltaTime / duration;
			
			Vector3 p = Vector3.Lerp(transform.localPosition, endPos, Mathf.SmoothStep(0f, 1f, t));
			
			transform.localPosition = new Vector3(
				Mathf.Round(p.x * 100f) / 100f, Mathf.Round(p.y * 100f) / 100f, p.z
			);

			yield return null;
		}
	}


	public void LocateAtPos (Vector2 pos) {
		transform.localPosition = new Vector3(pos.x, pos.y, -10);
	}

}
