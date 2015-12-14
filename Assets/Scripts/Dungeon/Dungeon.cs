using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (DungeonGenerator))]
[RequireComponent (typeof (Grid))]

public class Dungeon : MonoSingleton <Dungeon> {
	private AudioManager sfx;
	private Game game;
	private Hud hud;
	private Grid grid;
	private DungeonGenerator dungeonGenerator;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel = 0;


	void Awake () {
		sfx = AudioManager.instance;
		hud = Hud.instance;
		game = Game.instance;
		grid = Grid.instance;
		dungeonGenerator = DungeonGenerator.instance;
	}


	// =====================================================
	// Generate Dungeon
	// =====================================================

	public void GenerateDungeon (int direction = 0) {
		// Update current dungeon level
		currentDungeonLevel += direction;

		// Set random seed
		int seed;
		if (currentDungeonLevel > dungeonSeeds.Count - 1) {
			// Set a random seed if we are entering a new dungeon level
			seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
			dungeonSeeds.Add(seed);
		} else {
			// Recover a previously stored seed on current dungeon level
			seed = dungeonSeeds[currentDungeonLevel];
		}

		// Generate dungeon data
		dungeonGenerator.GenerateDungeon(seed);

		// Render dungeon on grid
		RenderDungeon(direction);

		sfx.Play("Audio/Sfx/Musical/gong", 0.2f, Random.Range(0.8f, 1.6f));
	}


	// =====================================================
	// Generate Dungeon Features
	// =====================================================

	public void RenderDungeon (int direction) {
		// init grid
		grid.InitializeGrid (dungeonGenerator.MAP_WIDTH, dungeonGenerator.MAP_HEIGHT);

		// Generate dungeon features
		GenerateDungeonFeatures(direction);

		// initialize player's vision after everything has been created
		grid.player.UpdateVision(grid.player.x, grid.player.y);

		// Update game turn
		game.UpdateGameTurn();

		// Log dungeon level
		int dlevel = currentDungeonLevel + 1;
		hud.Log("Welcome to dungeon level " + dlevel);
		hud.LogDungeon("DUNGEON " + dlevel);
	}


	private void GenerateDungeonFeatures (int direction) {
		// Generate architecture for each tree quad recursively
		ArchitectureGenerator architecture = new ArchitectureGenerator();
		architecture.GenerateArchitecture(dungeonGenerator.quadTree);

		// Generate stairs
		StairGenerator stairs = new StairGenerator();
		stairs.Generate();

		// If we cannot solve the level, we need to generate a different one
		if (!LevelIsSolvable()) {
			Debug.LogError("Dungeon level cannot be solved. Genrating again...");
			GenerateDungeon(direction);
			return;
		}

		// Generate player 
		PlayerGenerator player = new PlayerGenerator();
		Stair stair = direction == -1 ? grid.stairDown : grid.stairUp;
		player.GenerateAtPos(stair.x, stair.y);

		// Generate furniture
		FurnitureGenerator furniture = new FurnitureGenerator();
		furniture.Generate();

		// Generate monsters
		MonsterGenerator monsters = new MonsterGenerator();
		//monsters.Generate();
		monsters.GenerateSingle(typeof(Centaur));

		// Generate containers
		ContainerGenerator containers = new ContainerGenerator();
		containers.Generate();

		// Generate items
		ItemGenerator items = new ItemGenerator();
		items.Generate();
	}


	private bool LevelIsSolvable () {
		// if we could not place one of the stairs, level is not solvable
		if (grid.stairUp == null || grid.stairDown == null) {
			return false;
		}

		// if no available path from stair to stair, level is not solvable
		List<Vector2> path = Astar.instance.SearchPath(
			grid.stairUp.x, grid.stairUp.y, grid.stairDown.x, grid.stairDown.y
		);
		if (path.Count == 0) {
			return false;
		}

		// otherwise, we can solve the level
		return true;
	}


	// =====================================================
	// Navigate Dungeon Levels
	// =====================================================

	public void ExitLevel (int direction) {
		StartCoroutine(ExitLevelCoroutine(direction));
	}

	
	private  IEnumerator ExitLevelCoroutine (int direction) {
		game.CrossFadeRandomBgm();
		yield return new WaitForSeconds(0f);

		// fade out
		yield return StartCoroutine(hud.FadeOut(0.5f));
		
		// generate next dungeon level
		if (currentDungeonLevel + direction >= 0) {
			GenerateDungeon(direction);
		} else {
			grid.ResetGrid();
			hud.Log("You escaped the dungeon!");
			yield break;
		}

		// fade in
		yield return StartCoroutine(hud.FadeIn(0.5f));
	}

}
