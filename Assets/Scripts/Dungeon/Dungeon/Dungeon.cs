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
	private CaveGenerator caveGenerator;
	private DungeonType dungeonType;

	public List<int> dungeonSeeds = new List<int>();
	public int currentDungeonLevel;

	public static int seed;

	private int generationTries = 0;


	void Awake () {
		sfx = AudioManager.instance;
		hud = Hud.instance;
		game = Game.instance;
		grid = Grid.instance;
		dungeonGenerator = DungeonGenerator.instance;
		caveGenerator = CaveGenerator.instance;


	}

	// =====================================================
	// Enter Dungeon for the first time
	// =====================================================

	public void EnterDungeon () {
		currentDungeonLevel = -1;
		GenerateDungeon(1);
	}


	// =====================================================
	// Generate Dungeon
	// =====================================================

	public void GenerateDungeon (int direction = 0) {
		// Update current dungeon level
		currentDungeonLevel += direction;

		// Set the random seed
		SetRandomSeed(direction);

		// Set random parameters (dimensions, rooms, corridors, etc...)
		SetRandomParameters();

		// Choose dungeon type (dungeon / cave)
		int r = Random.Range(0, 100);
		dungeonType = r <= 60 ? DungeonType.Dungeon : DungeonType.Cave;

		//Generate dungeon type
		switch (dungeonType) {
		case DungeonType.Dungeon:
			dungeonGenerator.Generate(seed);
			break;
		case DungeonType.Cave:
			DungeonGenerator.instance.MAP_WIDTH +=  Random.Range(1, 9);;
			DungeonGenerator.instance.MAP_HEIGHT += Random.Range(1, 9);;
			caveGenerator.Generate(seed);
			break;
		}

		// Print dungeon final config
		print ((direction == 0 ? "Re-generating" : "Generating ") +
			dungeonType + " Level " + currentDungeonLevel + " [" +
			DungeonGenerator.instance.MAP_WIDTH + " x " + DungeonGenerator.instance.MAP_HEIGHT + "]" +
			" with seed " + seed
		);

		// Render dungeon on grid
		RenderDungeon(direction);

		// Play ready sound when all is done
		sfx.Play("Audio/Sfx/Musical/gong", 0.2f, Random.Range(0.8f, 1.6f));
	}


	private void SetRandomSeed (int direction = 0) {
		// Generate a new random seed, or get it from previously stored

		if (!LevelIsVisited()) {
			// Set a random seed if we are entering a new dungeon level
			seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
			dungeonSeeds.Add(seed);

		} else if (direction == 0) {
			// Update with a new random seed if we are regenerating this level for any reason
			seed = System.DateTime.Now.Millisecond * 1000 + System.DateTime.Now.Minute * 100;
			dungeonSeeds[currentDungeonLevel] = seed;

		} else {
			// Recover a previously stored seed on current dungeon level
			seed = dungeonSeeds[currentDungeonLevel];
		}

		// set new random seed
		//Random.seed = seed;
    Random.InitState(seed);
	}


	private void SetRandomParameters () {
		// Set random dimensions
		int w = Random.Range(12, 21);
		int h = Random.Range(12, 21);
		DungeonGenerator.instance.MAP_WIDTH = w;
		DungeonGenerator.instance.MAP_HEIGHT = h;

		// Set random parameters:
		// room size
		// int max = w > h ? w : h;
		// DungeonGenerator.instance.ROOM_MAX_SIZE = Random.Range(max / 2, max + 1);
		// DungeonGenerator.instance.ROOM_MIN_SIZE = Random.Range(max / 4, (max / 2) + 1);
		// room wall border (empty space between rooms in tiles)
		DungeonGenerator.instance.ROOM_WALL_BORDER = Random.Range(0, 2);
		// room ugly enabled (?)
		DungeonGenerator.instance.ROOM_UGLY_ENABLED = Random.Range(0, 2) == 1 ? true : false;
		// corridor max width
		//DungeonGenerator.instance.CORRIDOR_WIDTH = Random.Range(0, 3);
	}


	// =====================================================
	// Generate Dungeon Features
	// =====================================================

	public void RenderDungeon (int direction) {
		// init grid
		grid.InitializeGrid (dungeonGenerator.MAP_WIDTH, dungeonGenerator.MAP_HEIGHT);

		// Generate dungeon features
		bool success = GenerateDungeonFeatures(direction);

		if (!success) {
			return;
		}

		// initialize player's vision after everything has been created
		grid.player.UpdateVision(grid.player.x, grid.player.y);

		// Update game turn
		game.UpdateGameTurn();

		// Log dungeon level
		int dlevel = currentDungeonLevel + 1;
		hud.Log("Welcome to dungeon level " + dlevel);
		hud.LogDungeon("Dungeon " + dlevel);
	}


	private bool GenerateDungeonFeatures (int direction) {
		// Generate architecture for each tree quad recursively
		ArchitectureGenerator architecture = new ArchitectureGenerator();
		if (dungeonType == DungeonType.Dungeon) {
			architecture.GenerateArchitecture(dungeonGenerator.quadTree);
		} else {
			architecture.GenerateCaveArchitecture();
		}

		// Generate stairs
		StairGenerator stairs = new StairGenerator();
		stairs.Generate();

		// roll again if we could not place stairs (becasue there were not enough free tiles)
		if (grid.stairUp == null || grid.stairDown == null) {
			//Debug.LogWarning("Missing stairs, generating again...");
			GenerateAgain();
			return false;
		}

		// Generate player
		PlayerGenerator player = new PlayerGenerator();
		Stair stair = direction == -1 ? grid.stairDown : grid.stairUp;
		player.GenerateAtPos(stair.x, stair.y);

		// Generate furniture
		FurnitureGenerator furniture = new FurnitureGenerator();
		furniture.Generate(dungeonType == DungeonType.Dungeon ? 50 : 100, dungeonType == DungeonType.Dungeon ? 0.35f : 0.025f);

		// Generate containers
		ContainerGenerator containers = new ContainerGenerator();
		containers.Generate(dungeonType == DungeonType.Dungeon ? 50 : 100, dungeonType == DungeonType.Dungeon ? 0.15f : 0.01f);

		// Generate items
		ItemGenerator items = new ItemGenerator();
		items.Generate(dungeonType == DungeonType.Dungeon ? 50 : 100, dungeonType == DungeonType.Dungeon ? 0.15f : 0.01f);

		// roll again if we could not complete the dungeon after non-movable tiles were all placed
		if (!LevelIsSolvable()) {
			//Debug.LogWarning("Level not solvable, generating again...");
			GenerateAgain();
			return false;
		}

		// Generate monsters
		MonsterGenerator monsters = new MonsterGenerator();
		monsters.Generate(dungeonType == DungeonType.Dungeon ? 40 : 100, dungeonType == DungeonType.Dungeon ? 0.05f : 0.01f);
		//monsters.GenerateSingle("Zombie");

		return true;
	}


	private void GenerateAgain () {
		generationTries++;
		if (generationTries == 100) {
			Debug.LogError("Dungeon unsolvable after 100 iterations. Escaping application...");
			return;
		}

		//Debug.LogError("Dungeon level cannot be solved. Generating again...");
		GenerateDungeon(0);
	}


	private bool LevelIsSolvable () {
		// if we already visited this level, we know is solvable
		if (LevelIsVisited()) {
			return true;
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


	private bool LevelIsVisited () {
		return currentDungeonLevel <= dungeonSeeds.Count - 1;
	}


	// =====================================================
	// Navigate Dungeon Levels
	// =====================================================

	public void ExitLevel (int direction) {
		StartCoroutine(ExitLevelCoroutine(direction));
	}


	private  IEnumerator ExitLevelCoroutine (int direction) {
		game.CrossFadeRandomBgm();
		//yield return new WaitForSeconds(0f);

		// fade out
		yield return StartCoroutine(hud.FadeOut(0.5f));

		//Debug.Log("Exiting level " + currentDungeonLevel + " in direction " + direction);

		// generate next dungeon level
		if (currentDungeonLevel + direction >= 0) {
			generationTries = 0;
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
