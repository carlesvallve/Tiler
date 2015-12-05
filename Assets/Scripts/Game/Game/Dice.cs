using UnityEngine;
using System.Collections;

public class Dice {

	// 3d6+4

	public static int Roll (int min, int max, int maxDices = 1, int modifier = 0) {
		int value = 0;

		for (int i = 1; i <= maxDices; i++) {
			value += Random.Range(min, max + 1);
		}
		
		value += modifier;

		return value;
	}
	
}
