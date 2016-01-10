using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Home : MonoBehaviour {

	private AudioManager sfx;
	private Navigator navigator;
	private bool transitioning;

	void Awake () {
		// force framerate to 60 fps
		Application.targetFrameRate = 60;

		sfx = AudioManager.instance;
		DontDestroyOnLoad(sfx.gameObject);

		navigator = Navigator.instance;
		DontDestroyOnLoad(navigator.gameObject);

		sfx.Play("Audio/Bgm/Scenes/PeacefulTown", 0, 1f, true);
		sfx.Fade("Audio/Bgm/Scenes/PeacefulTown", 0.5f, 0.5f);

		// start label blink
		Transform startLabel = transform.Find("Button/TextStart");
		CanvasGroup group = startLabel.GetComponent<CanvasGroup>();
		StartCoroutine(Blink(group, 1f));
	}


	public void GotoScene (string sceneName) {
		if (transitioning) { return; }
		transitioning = true;

		StopAllCoroutines();

		sfx.Fade("Audio/Bgm/Scenes/PeacefulTown", 0, 1f);
		navigator.Open(sceneName, true);
	}


	public IEnumerator Blink (CanvasGroup group, float delay = 0) {
		group.alpha = 0;
		yield return new WaitForSeconds(delay);
		
		yield return StartCoroutine(Fade(group, 1, 0.5f, 0.25f));
		yield return StartCoroutine(Fade(group, 0, 0.25f, 2f));

		StartCoroutine(Blink(group, 0));
	}


	public IEnumerator Fade (CanvasGroup group, float value, float duration = 0.5f, float delay = 0) {
		yield return new WaitForSeconds(delay);

		float startValue = group.alpha;
		
		float elapsedTime = 0;
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;
			group.alpha = Mathf.Lerp(startValue, value, Mathf.SmoothStep(0f, 1f, t)); 
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		group.alpha = value;
	}

}
