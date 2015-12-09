using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Utils {

	public static string UppercaseFirst (string s) {
		if (string.IsNullOrEmpty(s)) { return string.Empty; }
		return char.ToUpper(s[0]) + s.Substring(1);
	}


	public static void Shuffle<T>(this IList<T> ts) {
		var count = ts.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i) {
			var r = UnityEngine.Random.Range(i, count);
			var tmp = ts[i];
			ts[i] = ts[r];
			ts[r] = tmp;
		}
	}


	public static string ListToString<T>(this IList<T> ts) {
		string str ="[";  
		for (int i = 0; i < ts.Count; i++) {
			str += ts[i];
			if (i < ts.Count -1) { str += ", "; }; 
		}
		str += "]";

		return str;
	}


	public static T RandomWeight<T> (List<T> list) where T : Tile {
		// Based on Brice's CasinoRouletteWheel algorithm
		// NOTE: the type of this list must be of a class that contains a 'weight' property
		
		// list of objects different 'weight' prop values
		/*List<MyObj> list = new List<MyObj>(); //[];
		for (int i = 0; i < 1000; i += 1) {
			list.Add(new MyObj(i));
		}*/

		var weightSum = 0;
		for (var i = 0; i < list.Count; i++) {
			weightSum += list[i].weight;
		}

		// console.log('weightSum', weightSum)

		int selection = 0;
		int remainingWeight = Random.Range(0, weightSum); //System.Random() * weightSum; // Math.Random() * weightSum;

		// console.log('remainingWeight', remainingWeight)
		remainingWeight -= list[selection].weight;
		while (remainingWeight > 0) {
			remainingWeight -= list[selection].weight;
			selection += 1;
		}

		Debug.Log ("selection: " + selection + "/" + list.Count + " " + list[selection].GetType());

		// strange that this ever happens...
		if (selection > list.Count -1) { 
			selection = list.Count - 1;
		}

		
		T obj = list[selection];

		// console.log('selection', selection)
		return obj;
	}


	// TODO implement a generic dictionary methods. This could totally work!
	/*public static T RandomWeight<T> (Dictionary<T, int> list) where T : System.Type {

		var weightSum = 0;
		for (var i = 0; i < list.Count; i++) {
			weightSum += list[i];.weight;
		}

		// console.log('weightSum', weightSum)

		int selection = 0;
		int remainingWeight = Random.Range(0, weightSum); //System.Random() * weightSum; // Math.Random() * weightSum;

		// console.log('remainingWeight', remainingWeight)
		remainingWeight -= list[selection].weight;
		while (remainingWeight > 0) {
			remainingWeight -= list[selection].weight;
			selection += 1;
		}

		Debug.Log ("selection: " + selection + "/" + list.Count + " " + list[selection].GetType());

		// strange that this ever happens...
		if (selection > list.Count -1) { 
			selection = list.Count - 1;
		}

		
		T obj = list[selection];

		// console.log('selection', selection)
		return obj;
	}*/
	
}


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
