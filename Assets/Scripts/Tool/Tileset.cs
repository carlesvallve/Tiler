using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


// SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel = 0);


public class Tileset : MonoBehaviour {

	public Sprite tilesetSprite;
	public int tileWidth = 8;
	public int tileHeight = 8;
	public bool showGrid = false;

	private Sprite source;
	private Sprite overlay;
	private GameObject grid;

	private List<TileRect> tiles = new List<TileRect>();

	private string tilesetName;
	private int currentTileX;
	private int currentTileY;


	void Awake () {
		Transform container = transform.Find("Container");

		source = InitSource(container);
		grid = InitGrid(container);
		overlay = InitOverlay(container);

		InitCamera();
		InitHud();
	}


	void Update () {
		UpdateUserInteraction();
	}

	//=============================================
	// Initialization
	// ============================================

	private void InitCamera () {
		Camera.main.orthographicSize = Screen.height / 4;
		Camera.main.transform.position = new Vector3(
			source.bounds.center.x, 
			source.bounds.center.y, 
			-10
		);
	}


	private Sprite InitSource (Transform parent) {
		GameObject go = new GameObject();
		go.name = "Source";
		go.transform.SetParent(parent, false);

		//GameObject go = transform.Find("Source").gameObject;
		
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = tilesetSprite;
		spriteRenderer.sortingOrder = 2;

		Sprite sprite = spriteRenderer.sprite;

		tilesetName = sprite.name;
		print ("Tileset: " + tilesetName);
		
		BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
		boxCollider.offset = (Vector2)sprite.bounds.center;
		boxCollider.size = (Vector2)sprite.bounds.extents * 2;

		/*go.transform.position = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 1, -Camera.main.transform.position.z)
		);*/

		return sprite;
	}


	private GameObject InitGrid (Transform parent) {
		GameObject go = new GameObject();
		go.name = "Grid";
		go.transform.SetParent(parent, false);
		go.SetActive(showGrid);

		int width = source.texture.width + 1;
		int height = source.texture.height + 1;

		// set dots texture
		Texture2D texture = new Texture2D (width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		DrawUtils.ClearTexture(texture);
		DrawUtils.DrawGrid(texture, tileWidth, tileHeight, new Color(0, 0, 0, 0.1f));

		texture.Apply ();

		// create sprite
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 2;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0f, 1f), 1);
		sprite.name = "DotsSprite";
		spriteRenderer.sprite = sprite;

		return go;
	}


	private Sprite InitOverlay (Transform parent) {
		GameObject go = new GameObject();
		go.name = "Overlay";
		go.transform.SetParent(parent, false);

		// set overlay texture
		Texture2D texture = new Texture2D (source.texture.width, source.texture.height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		DrawUtils.ClearTexture(texture);
		texture.Apply ();

		// create sprite
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 0;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		sprite.name = "OverlaySprite";
		spriteRenderer.sprite = sprite;

		return sprite;
	}


	private void DrawOverlay (int tileX, int tileY) {
		Texture2D texture = overlay.texture;
		
		DrawUtils.ClearTexture(texture);

		Vector2 coords;
		Color color;

		foreach (TileRect tile in tiles) {
			//DrawTileSelector(tile.x, tile.y, new Color(1, 1, 0, 0.8f));
			color = new Color(0, 1, 1, 0.8f);
			coords = DrawUtils.GetPixelPosInTexture(source.texture, tile.x, tile.y, tileWidth, tileHeight);
			DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth, tileHeight, color, true);
		}

		color = new Color(1, 0, 1, 0.8f);
		coords = DrawUtils.GetPixelPosInTexture(source.texture, tileX, tileY, tileWidth, tileHeight);

		DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth, tileHeight, color, true);

		texture.Apply();

		Transform go = transform.Find("Container/Overlay");
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		go.GetComponent<SpriteRenderer>().sprite = sprite;
	}


	private Texture2D DrawTileImage (int tileX, int tileY) {
		int width = tileWidth;
		int height = tileHeight;

		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		
		DrawUtils.ClearTexture(texture);
		
		Vector2 coords = DrawUtils.GetPixelPosInTexture(source.texture, tileX, tileY, tileWidth, tileHeight);

		Color[] colors = source.texture.GetPixels((int)coords.x, (int)coords.y, tileWidth, tileHeight);
		texture.SetPixels(0, 0, tileWidth, tileHeight, colors);
		
		texture.Apply();

		// create sprite and update hud image
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
		Image image = transform.Find("Hud/Header/Tile/TileImage").GetComponent<Image>();
		image.sprite = sprite;
		image.enabled = true;

		return texture;
	}


	//=============================================
	// Interaction
	// ============================================

	private void UpdateUserInteraction () {
		// escape if mouse is over hud
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		// get tile coords and update interface
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if(hit.collider != null) {
				Vector2 hitPos = new Vector2(hit.point.x - transform.position.x, hit.point.y - transform.position.y);

				Vector2 tileCoords = DrawUtils.GetTileCoordsInTexture(hitPos, tileWidth, tileHeight);
				int tileX = (int)tileCoords.x;
				int tileY = (int)tileCoords.y;

				DrawOverlay(tileX, tileY);
				DrawTileImage(tileX, tileY);
				UpdateTileInfo(tileX, tileY);

				// right mouse enables edit mode
				if (Input.GetMouseButtonDown(1)) {
					ShowPopupTileInfo(tileX, tileY);
				}
			}
		}
	}


	//=============================================
	// Hud
	// ============================================

	private void InitHud () {
		transform.Find("Hud/Popups/PopupTileInfo").gameObject.SetActive(false);
		transform.Find("Hud/Header/Tile/TileImage").GetComponent<Image>().enabled = false;
		transform.Find("Hud/Header/TileInfo").GetComponent<Text>().text = "unknown";
	}


	private void UpdateTileInfo (int tileX, int tileY) {
		Text info = transform.Find("Hud/Header/TileInfo").GetComponent<Text>();
		info.text = GetTileName(tileX, tileY) + "  (" + tileX + "," + tileY + ")" ;
	}


	public void ButtonToggleGrid () {
		showGrid = !showGrid;
		grid.SetActive(showGrid);
		transform.Find("Hud/Footer/ButtonGrid/Text").GetComponent<Text>().text = showGrid ? "GRID OFF" : "GRID ON";
	}


	public void ButtonSaveTile () {
		TileRect tile = GetTile(currentTileX, currentTileY);
		if (tile != null) {
			string path = Application.dataPath + "/Resources/Tilesets/" + tilesetName + "/" + tile.name + ".png";
			Texture2D texture = DrawTileImage(tile.x, tile.y);
			DrawUtils.SaveTextureToPng(texture, path);
		} else {
			Debug.LogError("No available tile to save.");
		}
	}


	public void ButtonSaveAll () {
		if (tiles.Count > 0) {
			foreach (TileRect tile in tiles) {
				Texture2D texture = DrawTileImage(tile.x, tile.y);
				string path = Application.dataPath + "/Resources/Tilesets/" + tilesetName + "/" + tile.name + ".png";
				DrawUtils.SaveTextureToPng(texture, path);
			}
		} else {
			Debug.LogError("No available tiles to save.");
		}
	}


	private void ShowPopupTileInfo (int tileX, int tileY) {
		currentTileX = tileX;
		currentTileY = tileY;

		Transform popup = transform.Find("Hud/Popups/PopupTileInfo");
		popup.gameObject.SetActive(true);

		string name = GetTileName(tileX, tileY);

		Text title = popup.Find("Box/Title").GetComponent<Text>();
		title.text = "Tile " + tileX + "," + tileY + ": " + name;

		if (name != "unknown") {
			popup.Find("Box/InputField").GetComponent<InputField>().text = name;
		} else {
			popup.Find("Box/InputField").GetComponent<InputField>().text = "unknown";
		}
	}


	public void AcceptPopupTileInfo (bool value) {
		Transform popup = transform.Find("Hud/Popups/PopupTileInfo");
		popup.gameObject.SetActive(false);
		if (!value) { return; }

		string name = popup.Find("Box/InputField").GetComponent<InputField>().text;

		if (name != "unknown") {
			SetTileData(currentTileX, currentTileY, name);
		}
	}

	//=============================================
	// Tile Data
	// ============================================

	private TileRect GetTile (int tileX, int tileY) {
		foreach (TileRect tile in tiles) {
			if (tile.x == tileX && tile.y == tileY) {
				return tile;
			}
		}

		return null; //"unknown";
	}


	private string GetTileName (int tileX, int tileY) {
		TileRect tile = GetTile(tileX, tileY);
		if (tile != null) { 
			return tile.name;
		} 
		return "unknown";
	}


	private void SetTileData (int tileX, int tileY, string name) {
		foreach (TileRect tile in tiles) {
			if (tile.x == tileX && tile.y == tileY) {
				tile.name = name;
				UpdateTileInfo(tileX, tileY);
				return;
			}
		}

		tiles.Add(new TileRect(tileX, tileY, name));
		UpdateTileInfo(tileX, tileY);
	}


	public void SaveTilesetData (string name) {
		// Note: your data can only be numbers and strings.
		JSONObject data = new JSONObject(JSONObject.Type.OBJECT);

		// tileset name
		data.AddField("name", name);

		// tileset tiles array
		JSONObject tileArr = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("tiles", tileArr);

		foreach (TileRect tile in tiles) {
			JSONObject tileData = new JSONObject(JSONObject.Type.OBJECT);

			tileData.AddField("name", tile.name);
			tileData.AddField("x", tile.x);
			tileData.AddField("y", tile.y);

			tileArr.Add(tileData["name"]);
			tileArr.Add(tileData["x"]);
			tileArr.Add(tileData["y"]);
		}
	}
}
