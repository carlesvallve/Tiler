using UnityEngine;
using System.Collections;

public class CreatureModule {

	protected Grid grid;
	protected AudioManager sfx;

	protected Creature me;


	public CreatureModule () {
		
	}


	protected void Init(Creature creature) {
		this.grid = Grid.instance;
		this.sfx = AudioManager.instance;
		this.me = creature;
	}
}
