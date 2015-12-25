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


	public static Color HexToRGB (int pColor) {
		Color color;
	   
		color.r = ((pColor * 0xFF0000) >> 16) / 255f;
		color.g = ((pColor * 0x00FF00) >> 8) / 255f;
		color.b = (pColor * 0x0000FF) / 255f;
		color.a = 1f;
	   
		return color;
	}


	public static Color HexToRgb(int hex) {
     //int bigint = System.ParseInt(hex, 16);

     float r = (hex >> 16) / 255f;
     float g = (hex >> 8) / 255f;
     float b = hex / 255f;
 
     return new Color(r, g, b); //r + "," + g + "," + b;
 }
}


