using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

	public static string UppercaseFirst (string s) {
		if (string.IsNullOrEmpty(s)) { return string.Empty; }
		return char.ToUpper(s[0]) + s.Substring(1);
    }
}
