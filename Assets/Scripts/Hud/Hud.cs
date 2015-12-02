using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoSingleton <Hud> {

	public float textSpeed = 0.025f;

	private Text turnText;
	private Text logText;

	private CanvasGroup overlayGroup;


	void Awake () {
		Canvas canvas = GetComponent<Canvas>();
		canvas.sortingLayerName = "Ui";
		canvas.sortingOrder = short.MaxValue;

		overlayGroup = transform.Find("Overlay").GetComponent<CanvasGroup>();

		turnText = transform.Find("Header/Turn/Text").GetComponent<Text>();
		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();
	}

	
	// ==============================================================
	// Logs
	// ==============================================================

	public void Log (string str) {
		WriteText(logText, str, textSpeed, false);
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

		if (dialogText == null) { yield break; }

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

		//group.alpha = 1;
		group.gameObject.SetActive(true);
		
		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(1, 0, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = 0;
		group.gameObject.SetActive(false);
	}


	public IEnumerator FadeOut(float duration, float delay = 0) {
		CanvasGroup group = overlayGroup;

		yield return new WaitForSeconds(delay);

		group.gameObject.SetActive(true);
		//group.alpha = 0;
		yield return null;
		
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
