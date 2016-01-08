using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class Camera2D : MonoBehaviour {

	public static Camera2D instance;

	// resolution
	public int pixelsPerUnit { get; set; }
	
	// panning
	public float panSpeed = 1f;
	private Vector3 lastMousePos;


	void Start () {
		instance = this;

		float ratio = ((float)Screen.width / (float)Screen.height);
		Debug.Log(Screen.width + " x " + Screen.height + " =  ratio " + ratio);

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer) {
			pixelsPerUnit = 72;
		} else {
			pixelsPerUnit = 32;
		}
			
		Camera.main.orthographicSize = (Screen.height / 2) / pixelsPerUnit;
	}


	void Update () {

		Camera.main.orthographicSize = (Screen.height / 2) / pixelsPerUnit;

		// apply panning
		if (Input.GetMouseButtonDown(1)) {
			lastMousePos = Input.mousePosition;
		} else if (Input.GetMouseButton(1)) {
			Vector2 delta = (Input.mousePosition - lastMousePos) * panSpeed;
			PanOrthoCamera(delta);
			lastMousePos = Input.mousePosition;
		}	
	}


	private void PanOrthoCamera (Vector2 delta) {
		// get final delta
		float pixelSizeAdjustment = 1f / pixelsPerUnit;
		delta *= pixelSizeAdjustment;

		// get final offset
		Vector2 offset = Vector2.Scale(delta, Camera.main.transform.localScale);

		// translate camera by offset
		Camera.main.transform.position -= (Vector3)offset;
	}


	public void CheckPlayerLimits (Player player) {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);

		float d = 2f * pixelsPerUnit;
		
		float marginLeft = d;
		float marginRight = d;
		float marginTop = d + Hud.instance.headerHeight;
		float marginBottom = d + Hud.instance.footerHeight;

		//Debug.Log("X: " + screenPos.x + ", Y: " + screenPos.y);

		if (screenPos.x < marginLeft || screenPos.x > Screen.width - marginRight || 
			screenPos.y < marginTop || screenPos.y > Screen.height - marginBottom) {
			CenterCamera(player);
		}
	}


	public void CenterCamera (Player player, bool interpolate = true) {
		if (player.state == CreatureStates.Descending) { 
			return; 
		}

		if (instance == null) {
			return;
		}

		StopAllCoroutines();

		if (interpolate) {
			StartCoroutine(MoveToPos(new Vector2(player.x, player.y)));
		} else {
			LocateAtPos(new Vector2(player.x, player.y));
		}
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
