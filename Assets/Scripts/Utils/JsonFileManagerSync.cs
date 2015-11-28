using UnityEngine;
using System.Collections;
using System.IO;

public class JsonFileManagerSync : MonoBehaviour {


	public static string GetPath (string fileName) {
		#if UNITY_EDITOR
			return Application.dataPath + "/Resources/" + fileName + ".json";
		#else
			return Application.persistentDataPath + "/" + fileName + ".json";  
		#endif
	}
	

	public static void SaveJsonFile (string fileName, JSONObject data) {
		// get file path
		string path = GetPath(fileName);

		Directory.CreateDirectory(Path.GetDirectoryName(path));

		// write data as a pretty multiline string
		string str = data.ToString(true);

		// write string to file
		using (FileStream fs = new FileStream(path, FileMode.Create)) {
			using (StreamWriter writer = new StreamWriter(fs)) {
				writer.Write(str);

				// refresh the editor
				#if UNITY_EDITOR
					UnityEditor.AssetDatabase.Refresh ();
				#endif
			}
		}
	}


	public static JSONObject LoadJsonFile (string fileName) {
		string path = GetPath(fileName);

		if (File.Exists(path)) {
			FileStream file = new FileStream (path, FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader( file );

			string str = null;
			str = sr.ReadToEnd();

			sr.Close();
			file.Close();

			JSONObject json = new JSONObject (str);
			return json;
		}

		return null;
	}


	public static bool DeleteJsonFile (string fileName) {
		string path = GetPath(fileName);

		if (File.Exists(path)) {
    		File.Delete(path);
    		return true;
		} else {
			return false;
		}
	}
}
