using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CaveGenerator : MonoSingleton <CaveGenerator> {

	public int seed = -1;

	public int[,] Map;
 
	public int MapWidth	{ get; set; }
	public int MapHeight { get; set; }
	public int PercentAreWalls { get; set; }


	public void Generate (int seed, int iterations = 4) {
		this.seed = seed;

		MapWidth = DungeonGenerator.instance.MAP_WIDTH;
		MapHeight = DungeonGenerator.instance.MAP_HEIGHT;
		PercentAreWalls = 45;
 
 		// fill map randomly
		RandomFillMap();

		// apply algorithm-1
		for (int i = 1; i <= 4; i++) {
			MakeCaverns1();
		}

		// apply algorithm-2
		for (int i = 1; i <= 4; i++) {
			MakeCaverns2();
		}
		
		PrintMap();
	}


	// =====================================================
	// Make Caverns
	// =====================================================

	public void MakeCaverns1 () {
		// By initilizing column in the outter loop, its only created ONCE
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

		// tile will turn to wall if if 
		// - neighbours in radius 1 are at least 5 
		// - neighbours in radius 2 are 2 or less
		// - otherwise, tile will turn to floor

		if (GetAdjacentWalls(x, y, 1) >= 5) { 
			return 1; 
		} 

		if (GetAdjacentWalls(x, y, 2) <= 2) {
			return 1;
		}
		
		return 0;
	}


	public int PlaceWallLogic2 (int x,int y) {

		// tile will turn wall if
		// - neighbours in radius 1 are at least 5 walls
		// - neighbours in radius 2 are 5 or less walls
		// - otherwise, tile will remain what it was before

		if (GetAdjacentWalls(x, y, 1) >= 5) {
			return 1;
		}

		if (GetAdjacentWalls(x, y, 2) <= 5) {
			return 1;
		}

		return Map[x,y];
	}


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
 
		if (Map[x,y] == 1) {
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

 
	/*public void BlankMap () {
		for(int column = 0,row = 0; row < MapHeight; row++) {
			for(column = 0; column < MapWidth; column++) {
				Map[column, row] = 0;
			}
		}
	}*/


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
		Debug.ClearDeveloperConsole();
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
			"\n" //Environment.NewLine
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
			returnString += "\n"; //Environment.NewLine;
		}

		return returnString;
	}
}



