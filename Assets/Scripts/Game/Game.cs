using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoSingleton <Game> {

	public static Assets assets;

	public int turn = 0;
	
	private AudioManager sfx;
	private List<string> bgmList;
	private string bgm1;
	private string bgm2;


	void Start () {
		assets = new Assets();
		SetBgm();
		InitGame();
	}


	private void InitGame () {
		// Generate dungeon level and render it in the game grid
		Dungeon dungeon = Dungeon.instance; 
		dungeon.GenerateDungeon();
	}


	public void UpdateTurn () {
		turn += 1;
		Hud.instance.LogTurn("TURN " + turn);
	}


	// =====================================================
	// Game Music
	// =====================================================

	private void SetBgm() {
		sfx = AudioManager.instance;

		bgmList = new List<string>() {
			"Audio/Bgm/Dungeon/Music/Alone",
			"Audio/Bgm/Dungeon/Music/GambooPiano",
			"Audio/Bgm/Dungeon/Music/Suspense",
			"Audio/Bgm/Dungeon/Music/Elementarywave",
			"Audio/Bgm/Dungeon/Music/Lifeline",
			"Audio/Bgm/Dungeon/Music/Suspense",

			"Audio/Bgm/Dungeon/Ambient/forest",
			"Audio/Bgm/Dungeon/Ambient/pulse",
			"Audio/Bgm/Dungeon/Ambient/reaktor",
			"Audio/Bgm/Dungeon/Ambient/space",
			"Audio/Bgm/Dungeon/Ambient/waterstream"
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
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 0.5f); }

		bgm1 = GetRandomBgm();
		sfx.Play(bgm1, 0, 1f, true);
		sfx.Fade(bgm1, 0.4f, 1f);

		bgm2 = GetRandomBgm();
		sfx.Play(bgm2, 0, 1f, true);
		sfx.Fade(bgm2, 0.4f, 1f);
	}

}







