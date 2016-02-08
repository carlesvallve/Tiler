using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraScalable : MonoBehaviour {
  
	// Use this for <span id="IL_AD1" class="IL_AD">initialization</span>
	void Update () {

		float TARGET_WIDTH = 1024; //Screen.width; //960.0f;
		float TARGET_HEIGHT = 768; //Screen.height; //540.0f;
		int PIXELS_TO_UNITS = 32; // 1:1 ratio of pixels to units

		float desiredRatio = TARGET_WIDTH / TARGET_HEIGHT;
		float currentRatio = (float)Screen.width/(float)Screen.height;

		if(currentRatio >= desiredRatio) {
			// Our resolution has plenty of width, so we just need to use the height to determine the camera size
			Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS;
		} else {
			// Our camera needs to zoom out further than just fitting in the height of the image.
			// Determine how much bigger it needs to be, then apply that to our original algorithm.
			float differenceInSize = desiredRatio / currentRatio;
			Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS * differenceInSize;
		}
	}
}
