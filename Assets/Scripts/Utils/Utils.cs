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


	public static MyObj RandomWeight(List<MyObj> list) {
		// list of objects different 'weight' prop values
		/*List<MyObj> list = new List<MyObj>(); //[];
		for (int i = 0; i < 1000; i += 1) {
			list.Add(new MyObj(i));
		}*/

		var weightSum = 0;
		for (var i = 0; i < 1000; i += 1) {
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

		MyObj obj = list[selection];

		// console.log('selection', selection)
		return obj;
	}
	
}


public class MyObj {
	public int value;
	public int weight;

	public MyObj (int i) {
		value = Random.Range(1, 1000);
		weight = (int)Mathf.Pow(1000 - i, 10);
	}
}
