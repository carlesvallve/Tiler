using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Dice {

	// =====================================================
	// Rpg dice roll ( i,e: 3d20+1 or 10d4-3 or 1d6)
	// =====================================================

	public static int Roll (string rpg) {
		if (rpg == null) {
			return 0;
		}
		
		char[] delimiters = new char[] { '+', '-', 'd' };
		string[] arr = rpg.Split(delimiters);

		int min = 1;
		int max = IntParseFast(arr[1]); // max value of a single dice
		int maxDices = IntParseFast(arr[0]); // max dices to use

		int modifier = arr.Length > 2 ? IntParseFast(arr[2]) : 0; // value of the modifier
		if (rpg.IndexOf("-") > -1) { modifier = -modifier; }

		//Debug.Log (maxDices + " dices from " + min + " to " + max + " with modifier " + modifier);

		// roll the dice
		return Roll(min, max, maxDices, modifier);
	}


	public static int Roll (int min, int max, int maxDices = 1, int modifier = 0) {
		int value = 0;

		for (int i = 1; i <= maxDices; i++) {
			value += Random.Range(min, max + 1);
		}
		
		value += modifier;

		return value;
	}


	public static int IntParseFast(string value) {
		int result = 0;
		for (int i = 0; i < value.Length; i++) {
			char letter = value[i];
			result = 10 * result + (letter - 48);
		}
		return result;
	}


	public static int GetMaxValue (string rpg) {
		char[] delimiters = new char[] { '+', '-', 'd' };
		string[] arr = rpg.Split(delimiters);

		int max = IntParseFast(arr[1]);
		int maxDices = IntParseFast(arr[0]);

		int modifier = arr.Length > 2 ? IntParseFast(arr[2]) : 0;
		if (rpg.IndexOf("-") > -1) { modifier = -modifier; }

		return max * maxDices + modifier;
	}


	// =====================================================
	// Probability Methods
	// =====================================================

	// returns a string key from a dictionary of weighted values

	public static string GetRandomStringFromDict (Dictionary<string, double> dict) {
		// generate a list of keys from our dictionary keys
		List<string> keys = new List<string>(dict.Keys);

		// generate a list of weights from our dictionary values
		List<double> values = new List<double>(dict.Values);

		// spin the roulette, passing an array of double values
		int index = SpinRoulette(values.ToArray());

		//Debug.Log (">>> " + index + " " + keys[index]);

		// get our type at index
		return keys[index];
	}

	// returns a class type from a dictionary of weighted class types

	public static System.Type GetRandomTypeFromDict (Dictionary<System.Type, double> dict) {
		// generate a list of types from our dictionary keys
		List<System.Type> types = new List<System.Type>(dict.Keys);

		// generate a list of weights from our dictionary values
		List<double> values = new List<double>(dict.Values);

		// spin the roulette, passing an array of double values
		int index = SpinRoulette(values.ToArray());

		//Debug.Log (">>> " + index + " " + types[index]);

		// get our type at index
		return types[index];
	}


	// returns a random tile from a list of tiles with weighted values

	public static Tile GetRandomTileFromList (List<Tile> list) {
		// generate a list of weights from our weight prop in our list of objects
		List<double> values = new List<double>();
		foreach (Tile item in list) {
			values.Add(item.interestWeight);
		}  

		// spin the roulette, passing an array of double values
		int index = SpinRoulette(values.ToArray());

		//Debug.Log (">>> " + index + " " + list[index]);

		// get our object at index
		return list[index];
	}


	// Casino Roulette, 
	// returns a random index number from an array of weighted probabilities

	// TODO: We are using System.Random here, 
	// so it will not respect the Random.Seed that we set for the dungeon level 
	// using UnityEngine.Random

	public static int SpinRoulette (double[] n) {

		//System.Random random = new System.Random();
		double total = 0;
		double[] c = new double[n.Length + 1];
		
		// Create cumulative values
		c[0] = 0;
		for (int i = 0; i < n.Length; i++) {
			c[i + 1] = c[i] + n[i];
			total += n[i];
		}

		// Create a random number between 0 and 1 and times by the total we calculated earlier.
		double r = Random.value * total; //Random.Range(0, total + 1); //random.NextDouble() * total;   

		// Don't use this - it's slower than the binary search below.  
		//int j; for (j = 0; j < c.Length; j++) if (c[j] > r) break; return j-1; 

		// Binary search for efficiency. 
		// Objective is to find index of the number just above r.
		int a = 0;
		int b = c.Length - 1;
		while (b - a > 1) {
			int mid = (a + b) / 2;
			if (c[mid] > r) b = mid;
			else a = mid;
		}

		return a;
	}

}


// Brice's javascript implementation

/*function casinoRouletteWheelMethod() {
	var myRndNumbers = [];

	for (var i = 0; i < 1000; i += 1) {
		myRndNumbers[i] = {
			value: Math.random(),
			weight: Math.pow(1000 - i, 10)
		}
	}

	var weightSum = 0;
	for (var i = 0; i < 1000; i += 1) {
		weightSum += myRndNumbers[i].weight;
	}

	// console.log('weightSum', weightSum)

	var selection = 0;
	var remainingWeight = Math.random() * weightSum;

	// console.log('remainingWeight', remainingWeight)
	remainingWeight -= myRndNumbers[selection].weight;
	while (remainingWeight > 0) {
		remainingWeight -= myRndNumbers[selection].weight;
		selection += 1;
	}

	// console.log('selection', selection)
	return selection;
}

var avg = 0;
var nTests = 10000;
for (var i = 0; i < nTests; i += 1) {
	avg += casinoRouletteWheelMethod();
}

console.log('avg', avg / nTests)*/

