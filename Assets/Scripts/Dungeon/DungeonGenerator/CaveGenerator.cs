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
		PercentAreWalls = 40;
 
		RandomFillMap();

		for (int i = 1; i <= 5; i++) {
			MakeCaverns1();
		}

		for (int i = 1; i <= 3; i++) {
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


	// cellular automata rules: 
	// a tile becomes a wall if it was a wall and 4 or more of its eight neighbors were walls, or if it was not a wall and 5 or more neighbors were walls

 
	public int PlaceWallLogic1 (int x,int y) {
		
		/*if (Map[x,y] == 1) {
			return 1;
		}*/

		if (GetAdjacentWalls(x, y, 1, 1) >= 5) { 
			return 1; 
		} 


		if (GetAdjacentWalls(x, y, 2, 2) <= 2) {
			return 1;
		}
		
		return Map[x,y];
	}


	public int PlaceWallLogic2 (int x,int y) {

		/*if (Map[x,y] == 1) {
			return 1;
		}*/

		if (GetAdjacentWalls(x, y, 1, 1) >= 5) {
			return 1;
		}


		return 0; //Map[x,y];
	}


	/*public int PlaceWallLogic (int x,int y, int iteration) {

		int numWalls = GetAdjacentWalls(x,y,1,1);
 
		if(Map[x,y]==1) {
			if (numWalls >= 4) {
				return 1;
			}

			if (numWalls < 2) {
				return 0;
			}
 
		} else {

			if (numWalls >= 5) {
				return 1;
			}
		}

		return 0;
	}*/


	


	public int GetAdjacentWalls (int x, int y, int scopeX, int scopeY) {
		int startX = x - scopeX;
		int startY = y - scopeY;
		int endX = x + scopeX;
		int endY = y + scopeY;
 
		int iX = startX;
		int iY = startY;
 
		int wallCounter = 0;
 
		for(iY = startY; iY <= endY; iY++) {
			for(iX = startX; iX <= endX; iX++) {
				//if(!(iX==x && iY==y)) {
					if(IsWall(iX,iY)) {
						wallCounter += 1;
					}
				//}
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

 
	public void BlankMap () {
		for(int column = 0,row = 0; row < MapHeight; row++) {
			for(column = 0; column < MapWidth; column++) {
				Map[column, row] = 0;
			}
		}
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



