using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	private AudioManager sfx;
	private Navigator navigator;
	private bool transitioning;


	void Awake () {
		sfx = AudioManager.instance;
		navigator = Navigator.instance;
		sfx.Play("Audio/Bgm/Scenes/GameOver", 0, 1f, true);
		sfx.Fade("Audio/Bgm/Scenes/GameOver", 0.25f, 0.5f);
	}


	public void GotoScene (string sceneName) {
		if (transitioning) { return; }
		transitioning = true;
		sfx.Fade("Audio/Bgm/Scenes/GameOver", 0, 1f);
		navigator.Open(sceneName, true);
	}
}
