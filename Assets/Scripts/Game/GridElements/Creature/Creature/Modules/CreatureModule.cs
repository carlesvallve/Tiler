using UnityEngine;
using System.Collections;

public class CreatureModule : MonoBehaviour {

	protected AudioManager sfx;
	protected Grid grid;
	protected Creature me;


	public virtual void Init (Creature creature) {
		this.sfx = AudioManager.instance;
		this.grid = Grid.instance;
		this.me = creature;
	}
}
