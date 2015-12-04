using UnityEngine;
using System.Collections;

public class Home : MonoBehaviour {

	private AudioManager sfx;
	private Navigator navigator;


	void Awake () {
		sfx = AudioManager.instance;
		DontDestroyOnLoad(sfx.gameObject);

		navigator = Navigator.instance;
		DontDestroyOnLoad(navigator.gameObject);
	}


	public void Navigate (string sceneName) {
		navigator.Open(sceneName, true);
	}
}
