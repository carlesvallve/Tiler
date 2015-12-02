using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoSingleton <Hud> {

	private Text logText;

	void Awake () {
		Canvas canvas = GetComponent<Canvas>();
		canvas.sortingLayerName = "Ui";
		canvas.sortingOrder = short.MaxValue;
		
		logText = transform.Find("Footer/Log/Text").GetComponent<Text>();
	}


	public void Log (string str) {
		logText.text = str;
	}
}
