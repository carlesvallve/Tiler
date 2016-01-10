using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

KNOWN BUGS
- sometimes, when monsters spawn items after dying, an item cannot be picked up
- sometimes, we attack a monster before he arrived to our adjacent tile
- when carrying a ranged weapon and shooting a monster, monsters dont react until some time has passed

ANDROID
- Set hud width/height ratio at 0.5 -> OK
- Implement cool custom font that accepts norml and bold
- interaction on finger-up, info and camera center in long press -> OK
- implement inventory icon on hud -> OK
- Reskin hood to be more full-screen like, bigger on the phone -> OK
- avoid interaction with game when we tap over hud -> OK
- display info when long tapping on inventory item


NEW EQUIPMENT SYSTEM
- generate new speadsheet -> OK
- generate equipment items using assetManager asset references -> OK
- place inventory items inside inventory container -> OK
- render equipment using the items we have equipped instead of random ones -> OK
- use alternative assets for boots and gloves -> OK
- pre-equip creatures with default equipment


CSV
- generate full list of monsters with more details and gradation of dangerousness and rarity
- generate full list of items with more details, gradation, and special effects
- generate some Randart generation to make some items magical artifacts
- generate list of spells and what they do exactly. Assign them to books.


AI
- monsters should turn afraid when hp bar is red -> OK
- afraid monsters should turn to fight if they have no other option -> OK
- monsters should equip collected equipment if is better than current -> OK

- monsters should use collected potions and food when hp is low
- intelligent monsters should have a chance to break containers and open chests

- monsters should elaborate more when evaluating objectives:
	- target player if agressivity roll is passed
		- agresivity will raise a lot if monster is attacked by the player
		- agressivity will decrease a lot if monster is severy injured
	- flee if agressivity is less than 0
		- check for potions or food around in paths not adjacent to the player
		- otherwise, flee
	- agressivity should be compared to attraction for some item
		- injured monsters are attracted to potions and food
		- animals are atracted by food
		- physical humanoids are attracted to better armour and weapons
		- magical humanoids are attracted to books


SPELLS
- generate list of spells
	- Red magic (Fire, destruction)
	- Blue Magic (Water, ilusion, changes)
	- Green magic (Eath, poison, body)
	- White magic (Healing, divination, cure)
	- Black magic (Necromancy, control, decadence)


SKILLS
- implement player skills
	- armour
	- weapon 
	- shield
	- magic
	- resistance
	- herbalism (potions)
	- sneaking
	- picklocking
	- traps


SKILL EXAMPLES
	Skills (Rogue)
		-Swords: 5
		-LightArmour: 6
		-Sneak: 2
		-Traps: 1
		-Picklock: 2
		-FireMagic: 1

	Skills (Berserk)
		- 2Handed: 6
		- MagicResistance: 4
		- LightArmour: 2
		- 

	Skills (Protector)
		- Spears: 3
		- Shield: 3
		- HeavyArmour: 5

	Skills (Mage)
		- FireMagic: 3
		- BloodMagic: 2
		- Divination : 1
		- Healing; 2
		- Staff: 2

	Skills (Monk)
		- Healing: 5
		- Divination: 3
		- Unarmed: 2
		- Potions: 2
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
		yield return new WaitForSeconds(1.0f);

		if (bgm1 != null) { sfx.Fade(bgm1, 0, 0.5f); }
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 0.5f); }
		
		Navigator.instance.Open("GameOver");
	}


	public void GameQuit () {
		if (bgm1 != null) { sfx.Fade(bgm1, 0, 0.5f); }
		if (bgm2 != null) { sfx.Fade(bgm2, 0, 0.5f); }
		
		Navigator.instance.Open("Home");
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
			sfx.Play(bgm1, 0, 1, true); //Random.Range(0.8f, 1.2f)
			sfx.Fade(bgm1, 0.25f, 1f);	
		}
		

		bgm2 = GetRandomBgm(ambientList, 60);
		if (bgm2 != null) {
			sfx.Play(bgm2, 0, Random.Range(0.8f, 1.2f), true);
			sfx.Fade(bgm2, 0.25f, 1f);
		}
	}

}







