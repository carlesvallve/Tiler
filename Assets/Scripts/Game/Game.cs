using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour {

	public static Assets assets;
	
	private AudioManager sfx;
	private List<string> bgmList;
	private string bgm1;
	private string bgm2;


	void Start () {
		assets = new Assets();
		SetBgm();
		InitGame();
	}


	private void SetBgm() {
		sfx = AudioManager.instance;

		bgmList = new List<string>() {
			"Audio/Bgm/Ambient/Alone-music",
			"Audio/Bgm/Ambient/GambooPiano-music",
			"Audio/Bgm/Ambient/PeacefulTown-music",
			"Audio/Bgm/Ambient/Suspense-music",

			"Audio/Bgm/Ambient/elementarywave-ambient",
			"Audio/Bgm/Ambient/forest-ambient",
			"Audio/Bgm/Ambient/lifeline-ambient",
			"Audio/Bgm/Ambient/pulse-ambient",

			"Audio/Bgm/Ambient/reaktor-ambient",
			"Audio/Bgm/Ambient/space-ambient",
			"Audio/Bgm/Ambient/suspense-ambient",
			"Audio/Bgm/Ambient/waterflow-ambient"
		};

		CrossFadeRandomBgm();
		/*// play level bgm
		bgm1 = GetRandomBgm();
		sfx.Play(bgm1, 0, 1f, true);
		sfx.Fade(bgm1, 0.4f, 1f);

		bgm2 = GetRandomBgm();
		sfx.Play(bgm2, 0, 1f, true);
		sfx.Fade(bgm2, 0.4f, 1f);*/
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


	private void InitGame () {
		// Generate dungeon level and render it in the game grid
		Dungeon dungeon = Dungeon.instance; 
		dungeon.GenerateDungeon();

		
	}

}







