using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tileset : MonoBehaviour {

	private Sprite source;
	private Sprite dots;
	private Sprite overlay;

	private int tileWidth = 8;
	private int tileHeight = 8;

	private List<TileRect> tiles = new List<TileRect>();


	void Awake () {
		source = InitSource();
		dots = InitDots();
		overlay = InitOverlay();

		Camera.main.transform.position = new Vector3(source.bounds.center.x, source.bounds.center.y, -10);
	}

	//=============================================
	// Initialization
	// ============================================

	private void ClearTexture (Texture2D texture, Color color) {
		// Set texture to transparent
		// --> There isn't any default color and never was; the pixels in a new Texture2D are always empty (i.e., undefined).
		Color32[] texColors = new Color32[texture.width * texture.height];
		for (int i = 0; i < texColors.Length; i++) { texColors[i] = color; } // Color.clear
		texture.SetPixels32(texColors);
	}


	private Sprite InitSource () {
		GameObject go = transform.Find("Source").gameObject;
		Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
		
		BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
		boxCollider.offset = (Vector2)sprite.bounds.center;
		boxCollider.size = (Vector2)sprite.bounds.extents * 2;

		return sprite;
	}


	private Sprite InitDots () {
		GameObject go = new GameObject();
		go.name = "Dots";
		go.transform.SetParent(transform, false);

		// set dots texture
		Texture2D texture = new Texture2D (source.texture.width, source.texture.height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		ClearTexture(texture, Color.clear);

		// paint dots
		Color color = new Color(0, 0, 0, 0.5f);
		for (int y = 0; y < texture.height; y += tileHeight) {
			for (int x = 0; x < texture.width; x += tileWidth) {
				texture.SetPixel (x, y, color);
			}
		}
		texture.Apply ();

		// create sprite
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		sprite.name = "DotsSprite";
		spriteRenderer.sprite = sprite;

		return sprite;
	}


	private Sprite InitOverlay () {
		GameObject go = new GameObject();
		go.name = "Overlay";
		go.transform.SetParent(transform, false);

		// set overlay texture
		Texture2D texture = new Texture2D (source.texture.width, source.texture.height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		ClearTexture(texture, Color.clear);
		texture.Apply ();

		// create sprite
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		sprite.name = "OverlaySprite";
		spriteRenderer.sprite = sprite;

		return sprite;
	}


	private void DrawSelector (int tileX, int tileY) {
		Texture2D texture = overlay.texture;
		
		ClearTexture(texture, Color.clear);

		int x = 0 + tileX * tileWidth;
		int y = 0 + tileY * tileHeight;
		Color color = new Color(1, 1, 0, 0.3f);

		print (x + " " + y);

		Color[] texColors = new Color[tileWidth * tileHeight];
		for (int i = 0; i < texColors.Length; i++) { texColors[i] = color; }

		// SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel = 0);
		int finalY = texture.height - y - tileHeight;
		texture.SetPixels(x, finalY, tileWidth, tileHeight, texColors);
		texture.Apply();

		Transform go = transform.Find("Overlay");
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 1);
		go.GetComponent<SpriteRenderer>().sprite = sprite;
	}


	private void DrawTileImage (int tileX, int tileY) {
		int width = tileWidth * 2;
		int height = tileHeight * 2;

		//Texture2D texture = new Texture2D (16, 16, TextureFormat.ARGB32, false);
		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		
		ClearTexture(texture, Color.magenta);
		texture.Apply();

		int x = 0 + tileX * tileWidth;
		int y = 0 + tileY * tileHeight;

		int finalX = x;
		int finalY = source.texture.height - y - tileHeight;

		print (y + " " + finalY);

        Color[] colors = source.texture.GetPixels(finalX, finalY, tileWidth, tileHeight);
        texture.SetPixels(tileWidth / 2, tileHeight / 2, tileWidth, tileHeight, colors);
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

    			DrawSelector(tileX, tileY);
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



	// the texture that i will paint with and the original texture (for saving)
	/*private Texture2D targetTexture, targetTexture, tmpTexture;


	private void PaintTexture () {
		originalTexture = source.texture;

		//setting temp texture width and height 
		tmpTexture = new Texture2D (originalTexture.width, originalTexture.height);

		//fill the new texture with the original one (to avoid "empty" pixels)
		for (int y =0; y < tmpTexture.height; y++) {
			for (int x = 0; x < tmpTexture.width; x++) {
				tmpTexture.SetPixel (x, y, originalTexture.GetPixel (x, y));
			}
		}

		print (tmpTexture.height);

		//filling a part of the temporary texture with the target texture 
		for (int y =0; y < tmpTexture.height - 40; y++) {
			for (int x = 0; x < tmpTexture.width; x++) {
				tmpTexture.SetPixel (x, y, targetTexture.GetPixel (x, y));
			}
		}

		//Apply 
		tmpTexture.Apply ();

		//change the object main texture 
		GetComponent<Renderer>().material.mainTexture = tmpTexture;
	}*/
}
