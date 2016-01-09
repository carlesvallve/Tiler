using UnityEngine;
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
	}


	public void GotoScene (string sceneName) {
		if (transitioning) { return; }
		transitioning = true;

		sfx.Fade("Audio/Bgm/Scenes/PeacefulTown", 0, 1f);
		navigator.Open(sceneName, true);
	}
}
