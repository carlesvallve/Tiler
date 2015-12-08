﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoSingleton <Game> {
	private Navigator navigator;
	private AudioManager sfx;
	private Grid grid;

	public int turn = 0;
	
	public Dictionary<string, ProceduralNameGenerator> gameNames;

	private List<string> bgmList;
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

		// recalculate player's vision
		grid.player.UpdateVision(grid.player.x, grid.player.y);
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
		string path = "Assets/Scripts/Utils/ProceduralNames/NameFiles/";

		gameNames = new Dictionary<string, ProceduralNameGenerator>() {
			{ "male", 	  new ProceduralNameGenerator(path + "Male.txt") },
			{ "female",   new ProceduralNameGenerator(path + "Female.txt") },
			{ "ukranian", new ProceduralNameGenerator(path + "Ukranian.txt") }
		};
	}


	// =====================================================
	// Game Music
	// =====================================================

	private void SetBgm() {
		bgmList = new List<string>() {
			"Audio/Bgm/Dungeon/Music/Alone",
			"Audio/Bgm/Dungeon/Music/GambooPiano",
			"Audio/Bgm/Dungeon/Music/Suspense",
			"Audio/Bgm/Dungeon/Music/Elementarywave",
			"Audio/Bgm/Dungeon/Music/Lifeline",
			"Audio/Bgm/Dungeon/Music/Suspense",

			"Audio/Bgm/Dungeon/Ambient/Forest",
			"Audio/Bgm/Dungeon/Ambient/Pulse",
			"Audio/Bgm/Dungeon/Ambient/Reaktor",
			"Audio/Bgm/Dungeon/Ambient/Space",
			"Audio/Bgm/Dungeon/Ambient/Waterstream"
		};

		CrossFadeRandomBgm();
	}


	private string GetRandomBgm () {
		string wav = null;

		while (true) {
			wav = bgmList[Random.Range(0, bgmList.Count)];
			if (!sfx.audioSources.ContainsKey(wav)) { break; }
		}

		return wav;
	}


	public void CrossFadeRandomBgm () {
		if (bgm1 != null) { sfx.Fade(bgm1, 0, 1f); }
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 1f); }

		bgm1 = GetRandomBgm();
		sfx.Play(bgm1, 0, 1f, true); // Random.Range(0.5f, 1.5f)
		sfx.Fade(bgm1, 0.5f, 1f);

		bgm2 = GetRandomBgm();
		sfx.Play(bgm2, 0, 1f, true); // Random.Range(0.5f, 1.5f)
		sfx.Fade(bgm2, 0.4f, 1f);
	}



	

}







