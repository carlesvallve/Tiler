using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoSingleton <Hud> {

	public float textSpeed = 0.025f;

	private Text turnText;
	private Text logText;
	private string lastLog;

	private CanvasGroup overlayGroup;

	private Transform world;
	public GameObject labelPrefab;


	void Awake () {
		Canvas canvas = GetComponent<Canvas>();
		canvas.sortingLayerName = "Ui";
		canvas.sortingOrder = short.MaxValue;

		overlayGroup = transform.Find("Overlay").GetComponent<CanvasGroup>();

		turnText = transform.Find("Header/Turn/Text").GetComponent<Text>();
		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();

		world = transform.Find("World");
	}

	// ==============================================================
	// UI labels
	// ==============================================================

	public void CreateLabel (Tile tile, string str, Color color, float duration = 1f, float startY = 24) {
		GameObject obj = (GameObject)Instantiate(labelPrefab);
		obj.transform.SetParent(world, false);
		obj.name = "Label";

		Text text = obj.transform.Find("Text").GetComponent<Text>();
		text.color = color;
		text.text = str;

		StartCoroutine(AnimateLabel(obj, tile, duration, startY));
	} 

	
	private IEnumerator AnimateLabel(GameObject obj, Tile tile, float duration, float startY) {
		StartCoroutine(FadeLabel(obj, tile, duration));

		Vector3 startPos = tile.transform.position;
		float endY = startY + 32;
		float t = 0;
		
		while (t <= 1) {
			t += Time.deltaTime / duration;

			float y = Mathf.Lerp(startY, endY, Mathf.SmoothStep(0f, 1f, t));
			Vector3 pos = Camera.main.WorldToScreenPoint(startPos) + Vector3.up * y;
			if (obj != null) { obj.transform.position = pos; }
			
			yield return null;
		}
	}


	private IEnumerator FadeLabel(GameObject obj, Tile tile, float duration) {
		CanvasGroup group = obj.GetComponent<CanvasGroup>();

		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.2f);
			group.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(duration * 0.4f);

		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.4f);
			group.alpha = (1 - t);
			yield return null;
		}

		yield return null;


		Destroy(obj);
	}
	
	// ==============================================================
	// Logs
	// ==============================================================

	public void Log (string str) {
		if (logText == null) { return; }
		if (str == lastLog) { return; }
		if (str == "") {
			logText.text = str;
			return;
		}

		WriteText(logText, str, textSpeed, false);

		lastLog = str;
	}


	public void LogTurn (string str) {
		turnText.text = str;
	}


	// ==============================================================
	// Text Animation
	// ==============================================================

	public void WriteText (Text dialogText, string str, float speed, bool preRender = false) {
		StartCoroutine(AnimateText(dialogText, str, speed));
	}	


	private IEnumerator AnimateText (Text dialogText, string str, float speed, bool preRender = false) {
		//print ("Animating text " + dialogText + " " +  str + " " + speed);

		if (preRender) { 
			dialogText.text = Invisible(str); 
		}

		float textTime = Time.time;
		int progress = 0;

		while (progress < str.Length) {
			while (textTime <= Time.time && progress < str.Length) {
				textTime = textTime + speed;
				progress++;

				dialogText.text = str.Substring(0, progress);
				if (preRender) {
					str += Invisible(str.Substring(progress));
				}
			}

			// if user interacts while writting the text, escape so all text is written instantly
			/*if (interactive && userInteracted == true) {
				break;
			}*/

			yield return null;
		}

		dialogText.text = str;
		lastLog = null;
	}


	private string Invisible (string raw) {
		return "<color=#00000000>" + raw + "</color>";
	}


	// ==============================================================
	// Overlay Fade In/Out
	// ==============================================================

	public IEnumerator FadeIn(float duration, float delay = 0) {
		CanvasGroup group = overlayGroup;

		yield return new WaitForSeconds(delay);

		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
	}


	public IEnumerator FadeOut(float duration, float delay = 0) {
		CanvasGroup group = overlayGroup;
		group.interactable = true;
		group.blocksRaycasts = true;

		yield return new WaitForSeconds(delay);
		
		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 1;	
	}
}
