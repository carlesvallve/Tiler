using UnityEngine;
using System.Collections;

public class DrawUtils : MonoBehaviour {


	public static void ClearTexture (Texture2D texture) {
		Color32[] texColors = new Color32[texture.width * texture.height];
		for (int i = 0; i < texColors.Length; i++) { texColors[i] = Color.clear; }
		texture.SetPixels32(texColors);
	}


	public static void FillTexture (Texture2D texture, Color color) {
		Color32[] texColors = new Color32[texture.width * texture.height];
		for (int i = 0; i < texColors.Length; i++) { texColors[i] = color; }
		texture.SetPixels32(texColors);
	}


	public static void DrawGrid (Texture2D texture, int tileWidth, int tileHeight, Color color) {
		for (int y = 0; y < texture.height; y += tileHeight) {
			DrawUtils.DrawLine(texture, 0, y, texture.width, y, color);
		}

		for (int x = 0; x < texture.width; x += tileWidth) {
			DrawUtils.DrawLine(texture, x, 0, x, texture.height, color);
		}
	}


	public static void DrawSquare (Texture2D texture, int x, int y, int width, int height, Color color, bool filled = false) {

		if (filled) {
			Color[] colors = new Color[width * height];
			for (int i = 0; i < colors.Length; i++) { colors[i] = color; }
			texture.SetPixels(x, y, width, height, colors);
		} else {
			
			DrawUtils.DrawLine(texture, x, y, x + width, y, color);
			DrawUtils.DrawLine(texture, x, y, x, y + height, color);
			DrawUtils.DrawLine(texture, x, y + height, x + width, y + height, color);
			DrawUtils.DrawLine(texture, x + width, y, x + width, y + height, color);
		}
	}


	public static void DrawLine (Texture2D texture, int x0, int y0, int x1, int y1, Color color) {
		int dy = (int)(y1-y0);
		int dx = (int)(x1-x0);
		int stepx, stepy;

		if (dy < 0) {
			dy = -dy; stepy = -1;
		} else { 
			stepy = 1;
		}

		if (dx < 0) {
			dx = -dx; stepx = -1;
		} else {
			stepx = 1;
		}

		dy <<= 1;
		dx <<= 1;

		float fraction = 0;

		texture.SetPixel(x0, y0, color);
		if (dx > dy) {
			fraction = dy - (dx >> 1);
			while (Mathf.Abs(x0 - x1) >= 1) {
				if (fraction >= 0) {
					y0 += stepy;
					fraction -= dx;
				}
				x0 += stepx;
				fraction += dy;
				texture.SetPixel(x0, y0, color);
			}
		}
		else {
			fraction = dx - (dy >> 1);
			while (Mathf.Abs(y0 - y1) >= 1) {
				if (fraction >= 0) {
					x0 += stepx;
					fraction -= dy;
				}
				y0 += stepy;
				fraction += dx;
				texture.SetPixel(x0, y0, color);
			}
		}
	}
}
