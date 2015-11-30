using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour {

	public static Assets assets;
	private AudioManager sfx;


	void Start () {
		assets = new Assets();
		InitGame();
	}


	private void InitGame () {
		// Generate dungeon level and render it in the game grid
		Dungeon dungeon = Dungeon.instance; 
		dungeon.GenerateDungeon();

		// play level bgm
		sfx = AudioManager.instance;
		sfx.Play("Audio/Bgm/Music/Alone", 0.6f, 1f, true);
		sfx.Play("Audio/Bgm/Ambient/BonusWind", 1f, 1f, true);
	}

}







