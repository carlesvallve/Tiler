using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class Camera2D : MonoBehaviour {

	public static Camera2D instance;

	// resolution
	int pixelsPerUnit = 32;
	
	// zoom
	//private float zoomSpeed = 0.5f;
	//private float minZoom = 0;
	//private float maxZoom = 1024 / 2;

	// panning
	public float panSpeed = 0.01f;
	private Vector3 lastMousePos;


	void Start () {
		instance = this;

		//maxZoom = ((Screen.height / 2) / pixelsPerUnit) * 2;
		//Camera.main.orthographicSize = maxZoom / 2;

		Camera.main.orthographicSize = (Screen.height / 2) / pixelsPerUnit;
	}


	void Update () {

		Camera.main.orthographicSize = (Screen.height / 2) / pixelsPerUnit;

		// apply scroll forward
		/*if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 * zoomSpeed);
		}

		// apply scoll back
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1 * zoomSpeed);
		}*/

		// apply panning
		if (Input.GetMouseButtonDown(1)) {
			lastMousePos = Input.mousePosition;
		} else if (Input.GetMouseButton(1)) {
			Vector2 delta = (Input.mousePosition - lastMousePos) * panSpeed;
			PanOrthoCamera(delta);
			lastMousePos = Input.mousePosition;
		}	

		// constrain camera to bg bounds
		//ConstrainToBounds();
	}


	/*private void ZoomOrthoCamera (Vector3 zoomTowards, float amount) {
		zoomTowards = Camera.main.transform.position;

		// Calculate how much we will have to move towards the zoomTowards position
		float multiplier = (1.0f / Camera.main.orthographicSize * amount);

		// Move camera
		transform.position += (zoomTowards - transform.position) * multiplier; 

		// Zoom camera
		Camera.main.orthographicSize -= amount;

		// Limit zoom
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
	}*/


	private void PanOrthoCamera (Vector2 delta) {
		// get final delta
		float pixelSizeAdjustment = 1f / pixelsPerUnit;
		delta *= pixelSizeAdjustment;

		// get final offset
		Vector2 offset = Vector2.Scale(delta, Camera.main.transform.localScale);

		// translate camera by offset
		Camera.main.transform.position -= (Vector3)offset;
	}


	/*private void ConstrainToBounds () {
		// calculate bg sprite bounds
		float vertExtent = Camera.main.orthographicSize;  
		float horzExtent = vertExtent * Screen.width / Screen.height;
		spriteBounds = bg.GetComponentInChildren<SpriteRenderer>();
		leftBound = (float)(horzExtent - spriteBounds.sprite.bounds.size.x / 2.0f);
		rightBound = (float)(spriteBounds.sprite.bounds.size.x / 2.0f - horzExtent);
		bottomBound = (float)(vertExtent - spriteBounds.sprite.bounds.size.y / 2.0f);
		topBound = (float)(spriteBounds.sprite.bounds.size.y  / 2.0f - vertExtent);

		Vector3 pos = Camera.main.transform.position; //new Vector3(target.position.x, target.position.y, transform.position.z);
		//print ("left: " + leftBound + " right: " + rightBound + " top: " + topBound + " bottom: " + bottomBound + " pos: " + pos);

		pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
		pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
		Camera.main.transform.position = pos;
	}*/


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
