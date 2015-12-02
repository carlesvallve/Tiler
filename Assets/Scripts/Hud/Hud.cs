using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoSingleton <Hud> {

	private Text turnText;
	private Text logText;

	void Awake () {
		Canvas canvas = GetComponent<Canvas>();
		canvas.sortingLayerName = "Ui";
		canvas.sortingOrder = short.MaxValue;

		turnText = transform.Find("Header/Turn/Text").GetComponent<Text>();
		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();
	}


	public void Log (string str) {
		logText.text = str;
	}

	public void LogTurn (string str) {
		turnText.text = str;
	}
}
