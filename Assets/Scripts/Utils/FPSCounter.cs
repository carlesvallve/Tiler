using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FpsCounter : MonoSingleton<FpsCounter> {

	public short sortOrder = short.MaxValue;

	private Canvas canvas;
	private Text textElement;
	private int currentFps;

	private const float FPS_MEASURE_PERIOD = 0.5f;
	private const string DISPLAY_FORMAT = "{0} FPS";
	private int fpsAccumulator = 0;
	private float fpsNextPeriod = 0;

	
	void Awake() {
		canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;

		textElement = gameObject.AddComponent<Text>();
		textElement.transform.SetParent(canvas.transform);
	}


	void Start() {
		// Sample period 
		fpsNextPeriod = Time.realtimeSinceStartup + FPS_MEASURE_PERIOD;

		// Text
		textElement.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		textElement.color = Color.yellow;
		textElement.fontSize = 10;
		textElement.alignment = TextAnchor.LowerRight;

		// Sorting layer
		canvas.sortingOrder = sortOrder;
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