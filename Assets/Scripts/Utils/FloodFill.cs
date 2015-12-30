using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FloodFill : MonoBehaviour {

	public static int[,] map;
	public static List<Point> area;
	public static List<List<Point>> areas;


	public static void Init (int[,] _map, int newColor, int oldColor) { 
		areas = new List<List<Point>>();
		area = new List<Point>();

		map = new int[_map.GetLength(0), _map.GetLength(1)];

		for(int column = 0, row = 0; row <= _map.GetLength(1) - 1; row++) {
			for(column = 0; column <= _map.GetLength(0) - 1; column++) {
				map[column, row] = _map[column, row];
			}
		}

		StartFromNextTile(newColor, oldColor);
	}


	public static void StartFromNextTile (int newColor, int oldColor) {
		Point p = GetNextFreeTile ();
		if (p == null) {
			//print ("Discovered " + areas.Count + " isolated areas");
			return;
		}

		area = new List<Point>();
		FloodFill8(p.x, p.y, newColor, oldColor);
		//print ("Discovered an area of " + area.Count + " tiles");
		areas.Add(area);

		StartFromNextTile(newColor, oldColor);
	}


	public static Point GetNextFreeTile () {
		for(int x = 0, y = 0; y <= map.GetLength(1) - 1; y++) {
			for(x = 0; x <= map.GetLength(0) - 1; x++) {
				if (map[x, y] == 0) { 
					return new Point(x, y); 
				}
			}
		}

		return null;
	}


	//Recursive 8-way floodfill, crashes if recursion stack is full
	public static void FloodFill8(int x, int y, int newColor, int oldColor) {
		if(x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && map[x,y] == oldColor && map[x,y] != newColor) {
			map[x,y] = newColor; //set color before starting recursion!

			area.Add(new Point(x, y));

			FloodFill8(x + 1, y,     newColor, oldColor);
			FloodFill8(x - 1, y,     newColor, oldColor);
			FloodFill8(x,     y + 1, newColor, oldColor);
			FloodFill8(x,     y - 1, newColor, oldColor);
			FloodFill8(x + 1, y + 1, newColor, oldColor);
			FloodFill8(x - 1, y - 1, newColor, oldColor);
			FloodFill8(x - 1, y + 1, newColor, oldColor);
			FloodFill8(x + 1, y - 1, newColor, oldColor);
		}    
	}

}
