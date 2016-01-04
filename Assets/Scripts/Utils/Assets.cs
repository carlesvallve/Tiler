using UnityEngine;
using System.Collections;



// DUNGEON

[System.Serializable]
public class AssetsDungeon {
	public AssetsDungeonArchitecture architecture;
	public Sprite[] container;
	public Sprite[] furniture;
}

[System.Serializable]
public class AssetsDungeonArchitecture {
	public Sprite[] door;
	public Sprite[] floor;
	public Sprite[] stair;
	public Sprite[] wall;
}


// ITEM

[System.Serializable]
public class AssetsItem {
	public Sprite[] book;
	public Sprite[] food;
	public Sprite[] potion;
	public Sprite[] tool;
	public Sprite[] treasure;
}


// MONSTER

[System.Serializable]
public class AssetsMonster {
	public Sprite[] animal;
	public Sprite[] humanoid;
}


// PLAYER

[System.Serializable]
public class AssetsPlayer {
	public AssetsArmour armour;
	public AssetsBody body;
	public AssetsCloak cloak;
	public AssetsHead head;
	public AssetsShield shield;
	public AssetsWeapon weapon;
}


[System.Serializable]
public class AssetsBody {
	public Sprite[] dwarf;
	public Sprite[] elf;
	public Sprite[] hobbit;
	public Sprite[] human;
}

[System.Serializable]
public class AssetsArmour {
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
public class AssetsCloak {
	public Sprite[] cloak;
}

[System.Serializable]
public class AssetsHead {
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
public class AssetsShield {
	public Sprite[] buckler;
	public Sprite[] kite;
	public Sprite[] large;
	public Sprite[] round;
}

[System.Serializable]
public class AssetsWeapon {
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

	public AssetsDungeon dungeon;
	public AssetsItem item;

	public AssetsMonster monster;
	public AssetsPlayer player;

}
