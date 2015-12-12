using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoSingleton <Hud> {

	public float textSpeed = 0.025f;

	private Text playerName;
	private Text playerInfo;
	private Text dungeonLevel;
	private Text gameTurn;

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
		world = transform.Find("World");


		playerName = transform.Find("Header/PlayerName/Text").GetComponent<Text>();
		playerInfo = transform.Find("Header/PlayerXp/Text").GetComponent<Text>();

		dungeonLevel = transform.Find("Header/DungeonLevel/Text").GetComponent<Text>();
		gameTurn = transform.Find("Header/Turn/Text").GetComponent<Text>();

		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();

		
	}

	// ==============================================================
	// UI labels
	// ==============================================================

	// TODO:  We probably want to create a 'Label' class to handle all these
	
	public void CreateLabel (Tile tile, string str, Color color, bool stick = false, float duration = 1f, float startY = 32) {
		GameObject obj = (GameObject)Instantiate(labelPrefab);
		obj.transform.SetParent(world, false);
		obj.name = "Label";

		Text text = obj.transform.Find("Text").GetComponent<Text>();
		text.color = color;
		text.text = str;

		StartCoroutine(AnimateLabel(tile, obj, stick, duration, startY));
		StartCoroutine(FadeLabel(tile, obj, duration));
	} 

	
	private IEnumerator AnimateLabel(Tile tile, GameObject obj, bool stick, float duration, float startY) {
		Vector3 startPos = tile.transform.position;
		float endY = startY + 32;
		float t = 0;
		
		while (t <= 1) {
			t += Time.deltaTime / duration;

			float y = Mathf.Lerp(startY, endY, Mathf.SmoothStep(0f, 1f, t));

			Vector3 pos = Camera.main.WorldToScreenPoint(
				tile != null && stick ? tile.transform.position : startPos
			) + Vector3.up * y;

			if (obj != null) { obj.transform.position = pos; } 
			
			yield return null;
		}
	}


	private IEnumerator FadeLabel(Tile tile, GameObject obj, float duration) {
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
	// Header
	// ==============================================================

	public void LogPlayerName (string str) {
		playerName.text = str;
	}

	public void LogPlayerInfo (string str) {
		playerInfo.text = str;
	}

	public void LogTurn (string str) {
		gameTurn.text = str;
	}


	public void LogDungeon (string str) {
		dungeonLevel.text = str;
	}


	// ==============================================================
	// footer
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
