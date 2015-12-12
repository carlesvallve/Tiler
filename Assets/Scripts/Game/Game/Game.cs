using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoSingleton <Game> {
	private Navigator navigator;
	private AudioManager sfx;
	private Grid grid;

	public int turn = 0;
	
	public Dictionary<string, ProceduralNameGenerator> gameNames;

	private List<string> musicList;
	private List<string> ambientList;
	private string bgm1;
	private string bgm2;


	void Start () {
		navigator = Navigator.instance; navigator.transform.Translate(Vector3.zero);
		sfx = AudioManager.instance;

		SetBgm();

		grid = Grid.instance;
		InitGame();
	}


	private void InitGame () {
		// initialize game name lists of each category
		InitializeGameNames();

		// Generate dungeon level and render it in the game grid
		Dungeon dungeon = Dungeon.instance; 
		dungeon.GenerateDungeon();

		// Game events
		grid.player.OnGameTurnUpdate += () => {
			grid.player.RegenerateHp();
			UpdateGameTurn();
		};

		grid.player.OnGameOver += () => {
			StartCoroutine(GameOver());
		};
	}


	public void UpdateGameTurn () {
		// update game turn
		turn += 1;
		Hud.instance.LogTurn("TURN " + turn);
	}


	public IEnumerator GameOver () {
		yield return new WaitForSeconds(0.5f);

		if (bgm1 != null) { sfx.Fade(bgm1, 0, 0.5f); }
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 0.5f); }
		
		Navigator.instance.Open("GameOver");
	}


	// =====================================================
	// Game Names
	// =====================================================

	private void InitializeGameNames () {
		string path = "Data/Names/";

		gameNames = new Dictionary<string, ProceduralNameGenerator>() {
			{ "male", 	  new ProceduralNameGenerator(path + "Male") },
			{ "female",   new ProceduralNameGenerator(path + "Female") },
			{ "ukranian", new ProceduralNameGenerator(path + "Ukranian") }
		};
	}


	// =====================================================
	// Game Music
	// =====================================================

	private void SetBgm() {
		musicList = new List<string>() {
			"Audio/Bgm/Dungeon/Music/guitar-rythm",
			"Audio/Bgm/Dungeon/Music/mystic-chamber",
			"Audio/Bgm/Dungeon/Music/piano-1",
			"Audio/Bgm/Dungeon/Music/piano-2",
			"Audio/Bgm/Dungeon/Music/piano-3",
			"Audio/Bgm/Dungeon/Music/piano-4",
			"Audio/Bgm/Dungeon/Music/piano-5",

			"Audio/Bgm/Dungeon/Music/synth-1",
			"Audio/Bgm/Dungeon/Music/synth-2",
			"Audio/Bgm/Dungeon/Music/synth-3",

			"Audio/Bgm/Dungeon/Music/Alone",
			"Audio/Bgm/Dungeon/Music/Elementarywave",
			"Audio/Bgm/Dungeon/Music/GambooPiano",
			"Audio/Bgm/Dungeon/Music/Lifeline",
		};

		ambientList = new List<string>() {
			"Audio/Bgm/Dungeon/Ambient/Forest",
			"Audio/Bgm/Dungeon/Ambient/Space",
			"Audio/Bgm/Dungeon/Ambient/Waterstream"
		};

		CrossFadeRandomBgm();
	}


	private string GetRandomBgm (List<string> list, int probability) {
		int r = Random.Range(1, 100);
		if (r > probability) {
			return null;
		}

		string wav = null;

		while (true) {
			wav = list[Random.Range(0, list.Count)];
			if (!sfx.audioSources.ContainsKey(wav)) { break; }
		}

		return wav;
	}


	public void CrossFadeRandomBgm () {
		if (bgm1 != null) { sfx.Fade(bgm1, 0, 1f); }
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 1f); }

		bgm1 = GetRandomBgm(musicList, 90);
		if (bgm1 != null) {
			sfx.Play(bgm1, 0, Random.Range(0.8f, 1.2f), true); 
			sfx.Fade(bgm1, 0.3f, 1f);	
		}
		

		bgm2 = GetRandomBgm(ambientList, 60);
		if (bgm2 != null) {
			sfx.Play(bgm2, 0, Random.Range(0.8f, 1.2f), true);
			sfx.Fade(bgm2, 0.3f, 1f);
		}
	}



	

}







