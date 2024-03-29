﻿using UnityEngine;
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

	private Transform container;
	private Sprite source;
	private Sprite overlay;
	private GameObject grid;

	private List<TileRect> tiles = new List<TileRect>();

	private string tilesetName;
	private int currentTileX;
	private int currentTileY;

	private Text info;


	void Awake () {
		container = transform.Find("Container");

		source = InitSource(container);
		grid = InitGrid(container);
		overlay = InitOverlay(container);

		InitCamera();
		InitHud();
		

		container.localPosition = Camera.main.ScreenToWorldPoint(
			new Vector3(10, - 58 + Screen.height, 10)
		);


		LoadTilesetData();
	}


	void Update () {
		UpdateUserInteraction();
	}

	//=============================================
	// Initialization
	// ============================================

	private void InitCamera () {
		Camera.main.orthographicSize = Screen.height / 8;
		//Camera.main.transform.position = new Vector3(source.bounds.center.x, source.bounds.center.y, -10);
	}


	private Sprite InitSource (Transform parent) {
		GameObject go = new GameObject();
		go.name = "Source";
		go.transform.SetParent(parent, false);

		//GameObject go = transform.Find("Source").gameObject;
		
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = tilesetSprite;
		spriteRenderer.sortingOrder = 1;

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
		spriteRenderer.sortingOrder = 0;

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
		spriteRenderer.sortingOrder = 2;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		sprite.name = "OverlaySprite";
		spriteRenderer.sprite = sprite;

		return sprite;
	}


	private void DrawOverlay (int tileX = -1, int tileY = -1) {
		Texture2D texture = overlay.texture;
		
		DrawUtils.ClearTexture(texture);

		Vector2 coords;
		Color color;

		// draw edited tiles
		foreach (TileRect tile in tiles) {
			color = new Color(0, 1, 0, 0.3f);
			coords = DrawUtils.GetPixelPosInTexture(source.texture, tile.x, tile.y, tileWidth, tileHeight);
			DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth - 1, tileHeight - 1, color, false);
			//DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth, tileHeight, color, true);
		}

		// draw selected tile
		if (tileX >= 0 && tileY >= 0) {
			color = new Color(1, 0, 0, 0.4f);
			coords = DrawUtils.GetPixelPosInTexture(source.texture, tileX, tileY, tileWidth, tileHeight);
			DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth - 1, tileHeight - 1, color, false);
			//DrawUtils.DrawSquare(texture, (int)coords.x, (int)coords.y, tileWidth, tileHeight, color, true);
		}
		
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
				Vector2 hitPos = new Vector2(
					hit.point.x - container.position.x, 
					hit.point.y - container.position.y
				);

				Vector2 tileCoords = DrawUtils.GetTileCoordsInTexture(hitPos, tileWidth, tileHeight);
				int tileX = (int)tileCoords.x;
				int tileY = (int)tileCoords.y;

				currentTileX = tileX;
				currentTileY = tileY;
				
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
		info = transform.Find("Hud/Footer/Info/Text").GetComponent<Text>();
		info.text = "";

		transform.Find("Hud/Popups/PopupTileInfo").gameObject.SetActive(false);
		transform.Find("Hud/Header/Tile/TileImage").GetComponent<Image>().enabled = false;
		transform.Find("Hud/Header/TileInfo").GetComponent<Text>().text = "unknown";
	}


	private void UpdateTileInfo (int tileX, int tileY) {
		Text tileInfo = transform.Find("Hud/Header/TileInfo").GetComponent<Text>();
		tileInfo.text = GetTileName(tileX, tileY) + "  (" + tileX + "," + tileY + ")" ;
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
	// Buttons
	// ============================================

	public void ButtonToggleGrid () {
		showGrid = !showGrid;
		grid.SetActive(showGrid);

		Text buttonText = transform.Find("Hud/Footer/Buttons/ButtonGrid/Text").GetComponent<Text>();
		buttonText.text = showGrid ? "GRID OFF" : "GRID ON";
	}


	public void ButtonSaveTile () {
		TileRect tile = GetTile(currentTileX, currentTileY);
		if (tile != null) {
			string path = Application.dataPath + "/Resources/Tilesets/" + tilesetName + "/Images/" + tile.name + ".png";
			Texture2D texture = DrawTileImage(tile.x, tile.y);
			DrawUtils.SaveTextureToPng(texture, path);

			info.text = "Tile " + tile.name + " has been saved.";
		} else {
			info.text = "No available tile to save.";
			Debug.LogError("No available tile to save.");
		}
	}


	public void ButtonSaveAll () {
		if (tiles.Count > 0) {
			foreach (TileRect tile in tiles) {
				string path = Application.dataPath + "/Resources/Tilesets/" + tilesetName + "/Images/" + tile.name + ".png";
				Texture2D texture = DrawTileImage(tile.x, tile.y);
				DrawUtils.SaveTextureToPng(texture, path);

				info.text = "All tiles has been saved.";
			}
		} else {
			info.text = "No available tiles to save.";
			Debug.LogError("No available tiles to save.");
		}
	}


	public void ButtonSaveTilesetData () {
		SaveTilesetData();
	}

	public void ButtonLoadTilesetData () {
		LoadTilesetData();
	}

	//=============================================
	// Tiles
	// ============================================

	private TileRect GetTile (int tileX, int tileY) {
		foreach (TileRect tile in tiles) {
			if (tile.x == tileX && tile.y == tileY) {
				return tile;
			}
		}

		return null;
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


	//=============================================
	// Json
	// ============================================

	public bool SaveTilesetData () {
		// create json data object
		JSONObject data = new JSONObject(JSONObject.Type.OBJECT);

		// tileset name
		data.AddField("name", tilesetName);

		// tileset tiles array
		JSONObject tileArr = new JSONObject(JSONObject.Type.ARRAY);
		data.AddField("tiles", tileArr);

		foreach (TileRect tile in tiles) {
			JSONObject tileData = new JSONObject(JSONObject.Type.OBJECT);

			tileData.AddField("name", tile.name);
			tileData.AddField("x", tile.x);
			tileData.AddField("y", tile.y);

			tileArr.Add(tileData);
		}

		//save json data to file
		try { 
			string path = "Tilesets/" + tilesetName + "/" + tilesetName;
			JsonFileManagerSync.SaveJsonFile(path, data);
			info.text = "Tileset data has been saved.";
			print ("Tileset data has been saved.");
			return true;
		} catch (System.Exception e) {
			info.text = "<color=#ff000>" + e +"</color";
			Debug.LogError(e);
			return false;
		}
	}


	public bool LoadTilesetData () {
		

		// load json data from file
		string path = "Tilesets/" + tilesetName + "/" + tilesetName;
		JSONObject json = JsonFileManagerSync.LoadJsonFile(path);

		if (json == null) {
			info.text = "Could not load " + path + ". File does not exist.";
			Debug.LogWarning("Could not load " + path + ". File does not exist.");
			return false;
		}

		info.text = "Tileset data was loaded successfully.";
		
		// generate tiles list from json tiles array
		JSONObject tileArr = json["tiles"];

		foreach (JSONObject tile in tileArr.list) {
			tiles.Add(new TileRect((int)(tile["x"].n), (int)(tile["y"].n), tile["name"].str));
		}

		// draw overlay to display the results
		DrawOverlay();

		return true;
	}
}
