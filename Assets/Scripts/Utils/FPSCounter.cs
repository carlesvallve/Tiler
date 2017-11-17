using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FPSCounter : MonoSingleton<FPSCounter> {

	private Text textElement;
	private int currentFps;

	private const float FPS_MEASURE_PERIOD = 0.5f;
	private const string DISPLAY_FORMAT = "{0} FPS";
	private int fpsAccumulator = 0;
	private float fpsNextPeriod = 0;


	void Awake() {
		textElement = GetComponent<Text>();

		if (!Debug.isDebugBuild) {
			gameObject.SetActive(false);
		}
	}


	void Start() {
		fpsNextPeriod = Time.realtimeSinceStartup + FPS_MEASURE_PERIOD;
	}


	void Update() {
		if (!Debug.isDebugBuild) {
			return;
		}

		// measure average frames per second
		fpsAccumulator++;

		if (Time.realtimeSinceStartup > fpsNextPeriod) {
			currentFps = (int) (fpsAccumulator/FPS_MEASURE_PERIOD);
			fpsAccumulator = 0;
			fpsNextPeriod += FPS_MEASURE_PERIOD;
			textElement.text = string.Format(DISPLAY_FORMAT, currentFps);
		}
	}
}
