﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
- spawning:
	- implement final spawning algorithm for monsters and items
	- spawned things should be stronger at higher dungeon levels

- inventory:
	- 2 handed items should be handled, appear in both inventory slots, etc
	- right clicking on inventory item should display item info

- ai:
	- monsters should turn afraid when hp bar is red -> OK
	- afraid monsters should turn to fight if they have no other option -> OK
	- monsters should use collected potions and food when hp is low
	- monsters should equip collected equipment if is better than current
	- intelligent monsters should have a chance to break containers and open chests
*/


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

		// load gmae external fata
		GameData gameData = new GameData();
		gameData.LoadAll();

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


	public IEnumerator WaitForTurnToEnd (Creature creature, float duration) {
		// Attack rate and movement rate should be independent:
		// A creature should always have 1 attack per turn for now,
		// so avoid thinking again if we are attacking
		if (creature.state == CreatureStates.Attacking) {
			//creature.stats.energy += creature.stats.energyRate;
			yield break;
		}

		// wait until monster has realize his action, then think again
		yield return new WaitForSeconds(duration);

		// think again
		creature.Think();
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
			sfx.Fade(bgm1, 0.25f, 1f);	
		}
		

		bgm2 = GetRandomBgm(ambientList, 60);
		if (bgm2 != null) {
			sfx.Play(bgm2, 0, Random.Range(0.8f, 1.2f), true);
			sfx.Fade(bgm2, 0.25f, 1f);
		}
	}



	

}







