using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel = 0);


public class Tileset : MonoBehaviour {

	public int tileWidth = 8;
	public int tileHeight = 8;
	public bool showGrid = false;

	private Sprite source;
	private GameObject grid;
	private Sprite overlay;

	private List<TileRect> tiles = new List<TileRect>();


	void Awake () {
		source = InitSource();
		grid = InitGrid();
		overlay = InitOverlay();

		InitCamera();
	}

	//=============================================
	// Initialization
	// ============================================

	private void InitCamera () {
		Camera.main.orthographicSize = Screen.height / 4;
		Camera.main.transform.position = new Vector3(source.bounds.center.x, source.bounds.center.y, -10);
	}

	private Sprite InitSource () {
		GameObject go = transform.Find("Source").gameObject;
		
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 1;

		Sprite sprite = spriteRenderer.sprite;
		
		BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
		boxCollider.offset = (Vector2)sprite.bounds.center;
		boxCollider.size = (Vector2)sprite.bounds.extents * 2;

		return sprite;
	}


	private GameObject InitGrid () {
		GameObject go = new GameObject();
		go.name = "Grid";
		go.transform.SetParent(transform, false);
		go.SetActive(showGrid);

		int width = source.texture.width + 1;
		int height = source.texture.height + 1;

		// set dots texture
		Texture2D texture = new Texture2D (width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		DrawUtils.ClearTexture(texture);
		DrawUtils.DrawGrid(texture, tileWidth, tileHeight, new Color(0, 0, 0, 0.5f));

		texture.Apply ();

		// create sprite
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 2;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0f, 1f), 1);
		sprite.name = "DotsSprite";
		spriteRenderer.sprite = sprite;

		return go;
	}


	private Sprite InitOverlay () {
		GameObject go = new GameObject();
		go.name = "Overlay";
		go.transform.SetParent(transform, false);

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


	private void DrawTileSelector (int tileX, int tileY) {
		Texture2D texture = overlay.texture;
		
		DrawUtils.ClearTexture(texture);

		int x = 0 + tileX * tileWidth;
		int y = 0 + tileY * tileHeight;
		int finalY = texture.height - y - tileHeight;

		DrawUtils.DrawSquare (texture, x, finalY, tileWidth, tileHeight, new Color(0, 0, 0, 0.5f), true);
		//DrawUtils.DrawSquare (texture, x + 1, finalY, tileWidth - 1, tileHeight - 1, new Color(1, 1, 0, 0.5f), true);
		//DrawUtils.DrawSquare (texture, x, finalY, tileWidth, tileHeight, new Color(0, 0, 0, 0.5f), false);

		texture.Apply();

		Transform go = transform.Find("Overlay");
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		go.GetComponent<SpriteRenderer>().sprite = sprite;
	}


	private void DrawTileImage (int tileX, int tileY) {
		int width = tileWidth;
		int height = tileHeight;

		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		
		DrawUtils.ClearTexture(texture);
		
		int x = 0 + tileX * tileWidth;
		int y = 0 + tileY * tileHeight;
		int finalX = x;
		int finalY = source.texture.height - y - tileHeight;

        Color[] colors = source.texture.GetPixels(finalX, finalY, tileWidth, tileHeight);
        texture.SetPixels(0, 0, tileWidth, tileHeight, colors);
        
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
        Image image = transform.Find("Hud/Header/TileImage").GetComponent<Image>();
        image.sprite = sprite;

        transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
	}


	//=============================================
	// Interaction
	// ============================================

	void Update () {

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if(hit.collider != null) {
				Vector2 hitPos = new Vector2(hit.point.x - transform.position.x, hit.point.y - transform.position.y);

				int pixelX = Mathf.FloorToInt(hitPos.x);
				int pixelY = Mathf.FloorToInt(-hitPos.y);
				int tileX = Mathf.RoundToInt(pixelX / tileWidth);
				int tileY = Mathf.RoundToInt(pixelY / tileHeight);
    			
    			print ("hitPos: " + hit.point + " pixelPos: "  + pixelX + "," + pixelY + " tilePos: " + tileX + "," + tileY);

    			DrawTileSelector(tileX, tileY);
    			DrawTileImage(tileX, tileY);
    			UpdtateTileInfo(tileX, tileY);
			}
		}

	}


	//=============================================
	// Hud
	// ============================================

	private void UpdtateTileInfo (int tileX, int tileY) {
		transform.Find("Hud/Header/TileInfo").GetComponent<Text>().text = "Tile [ " + tileX + ", " + tileY + " ]";
	}


	public void ButtonToggleGrid () {
		showGrid = !showGrid;
		grid.SetActive(showGrid);
	}

}
