using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace AssetLoader {

	// DUNGEON

	[System.Serializable]
	public class Dungeon {
		public DungeonArchitecture architecture;
		public Sprite[] container;
		public Sprite[] furniture;
	}

	[System.Serializable]
	public class DungeonArchitecture {
		public Sprite[] door;
		public Sprite[] floor;
		public Sprite[] stair;
		public Sprite[] wall;
	}

	// EQUIPMENT (Alternative assets)

	[System.Serializable]
	public class Equipment {
		public Sprite[] boots; 
		public Sprite[] cloak;
		public Sprite[] gloves;
	}

	// ITEM

	[System.Serializable]
	public class Item {
		public Sprite[] book;
		public Sprite[] food;
		public Sprite[] potion;
		public Sprite[] tool;
		public Sprite[] treasure;
	}

	// MONSTER

	[System.Serializable]
	public class Monster {
		public Animal animal;
		public Humanoid humanoid;
	}

	[System.Serializable]
	public class Animal {
		public Sprite[] bat;
		public Sprite[] bear;
		public Sprite[] cat;
		public Sprite[] chicken;
		public Sprite[] cow;
		public Sprite[] crab;
		public Sprite[] dog;
		public Sprite[] duck;
		public Sprite[] flie;
		public Sprite[] goat;
		public Sprite[] goose;
		public Sprite[] gorilla;
		public Sprite[] horse;
		public Sprite[] lion;
		public Sprite[] monkey;
		public Sprite[] mouse;
		public Sprite[] orangutan;
		public Sprite[] pig;
		public Sprite[] pigeon;
		public Sprite[] scorpion;
		public Sprite[] sheep;
		public Sprite[] turkey;
		public Sprite[] wolf;
	}


	[System.Serializable]
	public class Humanoid {
		public Sprite[] caveman;
		public Sprite[] centaur;
		public Sprite[] circus;
		public Sprite[] demon;
		public Sprite[] giant;
		public Sprite[] goblin;
		public Sprite[] hero;
		public Sprite[] knightdark;
		public Sprite[] knightlight;
		public Sprite[] lizardman;
		public Sprite[] merchant;
		public Sprite[] minotaur;
		public Sprite[] peasant;
		public Sprite[] pirate;
		public Sprite[] ratman;
		public Sprite[] satir;
		public Sprite[] snakeman;
		public Sprite[] troll;
		public Sprite[] vampire;
		public Sprite[] viking;
		public Sprite[] zombie;
	}


	// PLAYER

	[System.Serializable]
	public class Player {
		public Armour armour;
		public Body body;
		public Cloak cloak;
		public Head head;
		public Shield shield;
		public Weapon weapon;
	}


	[System.Serializable]
	public class Body {
		public Sprite[] dwarf;
		public Sprite[] elf;
		public Sprite[] hobbit;
		public Sprite[] human;
	}

	[System.Serializable]
	public class Armour {
		public Sprite[] belt;
		public Sprite[] chain;
		public Sprite[] cloth;
		public Sprite[] halfPlate;
		public Sprite[] leather;
		public Sprite[] leatherPlus;
		public Sprite[] plate;
		public Sprite[] robe;
	}

	[System.Serializable]
	public class Cloak {
		public Sprite[] cloak;
	}

	[System.Serializable]
	public class Head {
		public Sprite[] band;
		public Sprite[] cap;
		public Sprite[] crown;
		public Sprite[] hat;
		public Sprite[] helm;
		public Sprite[] hood;
		public Sprite[] horns;
		public Sprite[] wizard;
	}

	[System.Serializable]
	public class Shield {
		public Sprite[] buckler;
		public Sprite[] kite;
		public Sprite[] large;
		public Sprite[] round;
	}

	[System.Serializable]
	public class Weapon {
		public Sprite[] axe;
		public Sprite[] dagger;
		public Sprite[] mace;
		public Sprite[] ranged;
		public Sprite[] rod;
		public Sprite[] spear;
		public Sprite[] staff;
		public Sprite[] sword;
	}


	// ASSETS

	public class Assets : MonoBehaviour {

		public Dungeon dungeon;
		public Equipment equipment;
		public Item item;
		public Monster monster;
		public Player player;

		private static Dictionary<string, Sprite[]> categories;
		private static Dictionary<string, Sprite> assets;


		void Awake () {
			categories = new Dictionary<string, Sprite[]>();
			assets = new Dictionary<string, Sprite>();

			// dungeon

			AddCategory("dungeon/architecture/door", dungeon.architecture.door);
			AddCategory("dungeon/architecture/floor", dungeon.architecture.floor);
			AddCategory("dungeon/architecture/stair", dungeon.architecture.stair);
			AddCategory("dungeon/architecture/wall", dungeon.architecture.wall);
			AddCategory("dungeon/container", dungeon.container);
			AddCategory("dungeon/furniture", dungeon.furniture);

			// equipment

			AddCategory("equipment/boots", equipment.boots);
			AddCategory("equipment/cloak", equipment.cloak);
			AddCategory("equipment/gloves", equipment.gloves);

			// item

			AddCategory("item/book", item.book);
			AddCategory("item/food", item.food);
			AddCategory("item/potion", item.potion);
			AddCategory("item/tool", item.tool);
			AddCategory("item/treasure", item.treasure);

			// monster

			// animal
			AddCategory("monster/animal/bat", monster.animal.bat);
			AddCategory("monster/animal/bear", monster.animal.bear);
			AddCategory("monster/animal/cat", monster.animal.cat);
			AddCategory("monster/animal/chicken", monster.animal.chicken);
			AddCategory("monster/animal/cow", monster.animal.cow);
			AddCategory("monster/animal/crab", monster.animal.crab);
			AddCategory("monster/animal/dog", monster.animal.dog);
			AddCategory("monster/animal/duck", monster.animal.duck);
			AddCategory("monster/animal/flie", monster.animal.flie);
			AddCategory("monster/animal/goat", monster.animal.goat);
			AddCategory("monster/animal/goose", monster.animal.goose);
			AddCategory("monster/animal/gorilla", monster.animal.gorilla);
			AddCategory("monster/animal/horse", monster.animal.horse);
			AddCategory("monster/animal/lion", monster.animal.lion);
			AddCategory("monster/animal/monkey", monster.animal.monkey);
			AddCategory("monster/animal/mouse", monster.animal.mouse);
			AddCategory("monster/animal/orangutan", monster.animal.orangutan);
			AddCategory("monster/animal/pig", monster.animal.pig);
			AddCategory("monster/animal/pigeon", monster.animal.pigeon);
			AddCategory("monster/animal/scorpion", monster.animal.scorpion);
			AddCategory("monster/animal/sheep", monster.animal.sheep);
			AddCategory("monster/animal/turkey", monster.animal.turkey);
			AddCategory("monster/animal/wolf", monster.animal.wolf);

			// humanoid
			AddCategory("monster/humanoid/caveman", monster.humanoid.caveman);
			AddCategory("monster/humanoid/centaur", monster.humanoid.centaur);
			AddCategory("monster/humanoid/circus", monster.humanoid.circus);
			AddCategory("monster/humanoid/demon", monster.humanoid.demon);
			AddCategory("monster/humanoid/giant", monster.humanoid.giant);
			AddCategory("monster/humanoid/goblin", monster.humanoid.goblin);
			AddCategory("monster/humanoid/hero", monster.humanoid.hero);
			AddCategory("monster/humanoid/knightdark", monster.humanoid.knightdark);
			AddCategory("monster/humanoid/knightlight", monster.humanoid.knightlight);
			AddCategory("monster/humanoid/lizardman", monster.humanoid.lizardman);
			AddCategory("monster/humanoid/merchant", monster.humanoid.merchant);
			AddCategory("monster/humanoid/minotaur", monster.humanoid.minotaur);
			AddCategory("monster/humanoid/peasant", monster.humanoid.peasant);
			AddCategory("monster/humanoid/pirate", monster.humanoid.pirate);
			AddCategory("monster/humanoid/ratman", monster.humanoid.ratman);
			AddCategory("monster/humanoid/satir", monster.humanoid.satir);
			AddCategory("monster/humanoid/snakeman", monster.humanoid.snakeman);
			AddCategory("monster/humanoid/troll", monster.humanoid.troll);
			AddCategory("monster/humanoid/vampire", monster.humanoid.vampire);
			AddCategory("monster/humanoid/viking", monster.humanoid.viking);
			AddCategory("monster/humanoid/zombie", monster.humanoid.zombie);

			// player

			// armour
			AddCategory("player/armour/belt", player.armour.belt);
			AddCategory("player/armour/chain", player.armour.chain);
			AddCategory("player/armour/cloth", player.armour.cloth);
			AddCategory("player/armour/halfplate", player.armour.halfPlate);
			AddCategory("player/armour/leather", player.armour.leather);
			AddCategory("player/armour/leatherplus", player.armour.leatherPlus);
			AddCategory("player/armour/plate", player.armour.plate);
			AddCategory("player/armour/robe", player.armour.robe);

			// body
			AddCategory("player/body/dwarf", player.body.dwarf);
			AddCategory("player/body/elf", player.body.elf);
			AddCategory("player/body/hobbit", player.body.hobbit);
			AddCategory("player/body/human", player.body.human);

			// cloak
			AddCategory("player/cloak/cloak", player.cloak.cloak);

			// head
			AddCategory("player/head/band", player.head.band);
			AddCategory("player/head/cap", player.head.cap);
			AddCategory("player/head/crown", player.head.crown);
			AddCategory("player/head/hat", player.head.hat);
			AddCategory("player/head/helm", player.head.helm);
			AddCategory("player/head/hood", player.head.hood);
			AddCategory("player/head/horns", player.head.horns);
			AddCategory("player/head/wizard", player.head.wizard);

			// shield
			AddCategory("player/shield/buckler", player.shield.buckler);
			AddCategory("player/shield/kite", player.shield.kite);
			AddCategory("player/shield/large", player.shield.large);
			AddCategory("player/shield/round", player.shield.round);

			// weapon
			AddCategory("player/weapon/axe", player.weapon.axe);
			AddCategory("player/weapon/dagger", player.weapon.dagger);
			AddCategory("player/weapon/mace", player.weapon.mace);
			AddCategory("player/weapon/ranged", player.weapon.ranged);
			AddCategory("player/weapon/rod", player.weapon.rod);
			AddCategory("player/weapon/spear", player.weapon.spear);
			AddCategory("player/weapon/staff", player.weapon.staff);
			AddCategory("player/weapon/sword", player.weapon.sword);
		}


		private void AddCategory (string path, Sprite[] sprites) {
			// set category
			//print (path + " " + sprites.Length);
			categories[path] = sprites;

			// set assets
			foreach(Sprite sprite in sprites) {
				assets[path + "/" + sprite.name] = sprite;
			}
		}


		// TODO: Turn into generic methods <T>

		public static Sprite GetAsset (string pathName) {
			pathName = pathName.ToLower();

			if (!assets.ContainsKey(pathName)) {
				Debug.LogError("Asset not found. Key " + pathName + " doesnt exist.");
				return null;
			}

			return assets[pathName];
		}


		public static Sprite[] GetCategory (string pathName) {
			pathName = pathName.ToLower();

			if (!categories.ContainsKey(pathName)) {
				Debug.LogError("Category not found. Key " + pathName + " doesnt exist.");
				return null;
			}

			return categories[pathName];
		}

	}

}
