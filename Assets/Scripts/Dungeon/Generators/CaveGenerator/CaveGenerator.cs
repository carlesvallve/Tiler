using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CaveGenerator : MonoSingleton <CaveGenerator> {

	// The Random Seed
	public int seed = -1;

	public int[,] Map;
	public int MapWidth	{ get; set; }
	public int MapHeight { get; set; }
	public int PercentAreWalls { get; set; }

	public List<List<Point>> caverns;


	public void Generate (int seed) {
		// set and store random seed
		this.seed = seed;

		MapWidth = DungeonGenerator.instance.MAP_WIDTH;
		MapHeight = DungeonGenerator.instance.MAP_HEIGHT;
		PercentAreWalls = 35; // 35
 
 		// fill map randomly
		RandomFillMap();

		// apply algorithm-1
		for (int i = 1; i <= 5; i++) {
			MakeCaverns1();
		}

		// apply algorithm-2
		for (int i = 1; i <= 3; i++) {
			MakeCaverns2();
		}

		// connect isolated areas
		ConnectAreas();

		// remove inner walls
		RemoveInnerWalls();

		//PrintMap();
	}


	// =====================================================
	// Connect cavern areas
	// =====================================================

	private void ConnectAreas () {
		// Get isolated cavern areas by floodfill algorithm
		FloodFill.Init(Map, 1, 0);

		// connect the areas by digging paths between each area random tile
		List<List<Point>> areas = FloodFill.areas;
		caverns = areas;

		if (areas.Count > 0) {
			for (int i = 0; i < areas.Count; i++) {

				// get a random tile on this area
				List<Point> area = areas[i];
				Point p1 = area[Random.Range(0, area.Count)];

				// get a random tile on next area
				Point p2 = null;
				if (i < areas.Count -1) {
					List<Point> nextArea = areas[i + 1];
					p2 = nextArea[Random.Range(0, nextArea.Count)];
				} /*else {
					List<Point> nextArea = areas[0];
					p2 = nextArea[Random.Range(0, nextArea.Count)];
				}*/

				// dig a random path between the 2 points
				if (p2 != null) {
					DigPath(p1, p2);
				}
			}
		}
	}


	private void DigPath(Point p1, Point p2) {
		int c = 0;

		while (true) {
			// get a random cardinal direction from current point to goal
			int dx = (int)Mathf.Sign(p2.x - p1.x);
			int dy = (int)Mathf.Sign(p2.y - p1.y);
			int r = Random.Range(0, 100);
			if (r <= 50) { dx = 0; } else { dy = 0; }

			// dig corridor in direction
			p1 = new Point (p1.x + dx, p1.y + dy);

			if (IsWall(p1.x, p1.y)) {
				Map[p1.x, p1.y] = 2;
			}
			

			// escape if we arrived to goal
			if (p1.x == p2.x && p1.y == p2.x) {
				break;
			}

			// escape if something went wrong
			c++; if (c == 1000) { 
				Debug.Log("Problems while digging a path. Escaping...");
				break; 
			}
		}
	}



	// note: not in use
	// discarded in benefit of connecting areas by digging corridors
	private void RemoveIsolatedSmallAreas () {
		

		// Get isolated caver areas by floodfill algorithm
		FloodFill.Init(Map, 1, 0);

		// Remove biggest area from area list, since we want to keep it
		int c = 0;
		List<Point> myArea = null;
		foreach (List<Point> area in FloodFill.areas) {
			if (area.Count > c) {
				myArea = area;
				c = area.Count;
			}
		}

		FloodFill.areas.Remove(myArea);
		print (FloodFill.areas.Count + " areas to remove. Biggest area of " + myArea.Count + " tiles");

		// remove smallest isolated areas (turn them to walls)
		// TODO: It may be better if we could dig a corridor from area to area with astar...
		foreach (List<Point> area in FloodFill.areas) {
			foreach (Point p in area) {
				Map[p.x, p.y] = 1;
			}
		}
	}


	public void RemoveInnerWalls () {
		for(int x = 0, y = 0; y <= MapHeight - 1; y++) {
			for(x = 0; x <= MapWidth - 1; x++) {
				if (GetAdjacentWalls(x, y, 1) >= 9) {
					Map[x, y] = 3;
				}
			}
		}
	}


	// =====================================================
	// Make Caverns
	// =====================================================

	public void MakeCaverns1 () {
		for(int column = 0, row = 0; row <= MapHeight - 1; row++) {
			for(column = 0; column <= MapWidth - 1; column++) {
				Map[column,row] = PlaceWallLogic1(column,row);
				
			}
		}
	}

	public void MakeCaverns2 () {
		for(int column = 0, row = 0; row <= MapHeight - 1; row++) {
			for(column = 0; column <= MapWidth - 1; column++) {
				Map[column,row] = PlaceWallLogic2(column,row);
			}
		}
	}


	public int PlaceWallLogic1 (int x,int y) {
		// 5 neighbours or more in 1 radius, is wall
		if (GetAdjacentWalls(x, y, 1) >= 5) { 
			return 1; 
		} 

		// one neighbour or less in 2 radius, is floor
		if (GetAdjacentWalls(x, y, 2) <= 1) {
			return 0;
		}
		
		return Map[x,y];
	}


	public int PlaceWallLogic2 (int x,int y) {
		// 4 neighbours or less in 2 radius, is floor
		if (GetAdjacentWalls(x, y, 2) <= 4) {
			return 0;
		}

		return Map[x, y];
	}


	// =====================================================
	// GetWalls
	// =====================================================

	public int GetAdjacentWalls (int x, int y, int radius) {
		int startX = x - radius;
		int startY = y - radius;
		int endX = x + radius;
		int endY = y + radius;
 
		int iX = startX;
		int iY = startY;
 
		int wallCounter = 0;
 
		for(iY = startY; iY <= endY; iY++) {
			for(iX = startX; iX <= endX; iX++) {	
				if(IsWall(iX,iY)) {
					wallCounter += 1;
				}
			}
		}

		// returns all walls counting ourselves (0 to 9)
		return wallCounter;
	}

 
	bool IsWall (int x, int y) {
		// Consider out-of-bound a wall
		if (IsOutOfBounds(x,y)) {
			return true;
		}
 
		if (Map[x,y] == 1 || Map[x,y] == 3) {
			return true;
		}
 
		if (Map[x,y] == 0) {
			return false;
		}

		return false;
	}
 

	bool IsOutOfBounds(int x, int y) {
		if( x<0 || y<0 ) {
			return true;
		} else if (x > MapWidth-1 || y > MapHeight-1) {
			return true;
		}

		return false;
	}

 
	// =====================================================
	// Random Fill Map
	// =====================================================

	public void RandomFillMap () {
		// New, empty map
		Map = new int[MapWidth, MapHeight];
 
		int mapMiddle = 0; // Temp variable

		for (int column = 0,row=0; row < MapHeight; row++) {
			for (column = 0; column < MapWidth; column++) {
				// If coordinants lie on the the edge of the map (creates a border)
				if(column == 0) {
					Map[column,row] = 1;
				} else if (row == 0) {
					Map[column,row] = 1;
				} else if (column == MapWidth-1) {
					Map[column,row] = 1;
				} else if (row == MapHeight-1) {
					Map[column,row] = 1;
				}
				// Else, fill with a wall a random percent of the time
				else {
					mapMiddle = (MapHeight / 2);
 
					if(row == mapMiddle) {
						Map[column,row] = 0;
					} else {
						Map[column,row] = RandomPercent(PercentAreWalls);
					}
				}
			}
		}
	}

 
	int RandomPercent (int percent) {
		if (percent >= Random.Range(1, 101)) {
			return 1;
		}

		return 0;
	}


	// =====================================================
	// Print cave
	// =====================================================

	public void PrintMap () {
		Debug.Log(MapToString());
	}


	string MapToString () {
		string[] arr = new string[] {
			"Width:",
			MapWidth.ToString(),
			"\tHeight:",
			MapHeight.ToString(),
			"\t% Walls:",
			PercentAreWalls.ToString(),
			"\n"
		};

		string returnString = System.String.Join(" ", arr);

		List<string> mapSymbols = new List<string>();
		mapSymbols.Add("o");
		mapSymbols.Add("^");
		//mapSymbols.Add("+");
 
		for (int column = 0,row = 0; row < MapHeight; row++ ) {
			for (column = 0; column < MapWidth; column++ ) {
				returnString += mapSymbols[Map[column,row]];
			}
			returnString += "\n";
		}

		return returnString;
	}
}



