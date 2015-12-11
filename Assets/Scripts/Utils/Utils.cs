using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;


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


	public static void DebugList<T>(this IList<T> ts) {
		Debug.Log (ListToString(ts));
	}


	public static string GetStringPrepositions (string str) {
		if (str == "a" || str == "e" || str == "i" || str == "o" || str == "u") {
			return "an";
		} else {
			return "a";
		}
	}
}


