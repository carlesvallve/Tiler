using UnityEngine;
using System.Collections;
using System.IO;


public class DungeonExportPNG : MonoBehaviour {

	private DungeonGenerator generator;
	private Texture2D dungeonTexture;

	// =====================================================
	// Render dungeon in a texture and save it as a png file
	// =====================================================

	public void Export (string path) {
		generator = GetComponent<DungeonGenerator>();
		
		dungeonTexture = DungeonToTexture();
		TextureToFile(dungeonTexture, path);
	}

	private Texture2D DungeonToTexture() {
		Texture2D texOutput = new Texture2D((int)(generator.MAP_WIDTH), (int)(generator.MAP_HEIGHT), TextureFormat.ARGB32, false);
		PaintDungeonTexture(ref texOutput);
		texOutput.filterMode = FilterMode.Point;
		texOutput.wrapMode = TextureWrapMode.Clamp;
		texOutput.Apply();
		return texOutput;
	}


	private void PaintDungeonTexture(ref Texture2D t) {
		for (int i = 0; i < generator.MAP_WIDTH; i++) {
			for (int j = 0; j < generator.MAP_HEIGHT; j++) {
				switch (generator.tiles[j,i].id) {
				case DungeonTileType.EMPTY:
					t.SetPixel(i,j,Color.black);
					break;
				case DungeonTileType.ROOM:
					t.SetPixel(i,j,Color.white);
					break;
				case DungeonTileType.CORRIDOR:
					t.SetPixel(i,j,Color.grey);
					break;
				case DungeonTileType.WALL:
					t.SetPixel(i,j,Color.blue);
					break;
				case DungeonTileType.WALLCORNER:
					t.SetPixel(i,j,Color.blue);
					break;
				}
			}
		}
	}

	
	// Export a texture to a file
	private void TextureToFile(Texture2D t, string fileName) {
		string path = null;

		#if UNITY_EDITOR
			path = Application.dataPath + "/Resources/" + fileName + ".png";
		#else
			path = Application.persistentDataPath + "/" + fileName + ".png";  
		#endif

		print ("Saving dungeon texture at " + path);

		Directory.CreateDirectory(Path.GetDirectoryName(path));

		byte[] bytes = t.EncodeToPNG();
		FileStream myFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		myFile.Write(bytes,0,bytes.Length);
		myFile.Close();

		// refresh the editor
		#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
		#endif
	}
}
