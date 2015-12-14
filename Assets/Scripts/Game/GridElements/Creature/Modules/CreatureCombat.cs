using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CreatureCombat : CreatureModule {


	public CreatureCombat (Creature creature) {
		Init(creature);
	}


	// =====================================================
	// Combat Outcome
	// =====================================================

	public bool ResolveCombatOutcome (Creature attacker, Creature defender) {
		// resolve combat outcome
		int attack = attacker.stats.attack + Dice.Roll("1d8+2");
		int defense = defender.stats.defense + Dice.Roll("1d6+1");

		// hit
		if (attack > defense) {

			// damage = str + weapon damage dice
			string weaponDamage = attacker.stats.weapon != null ? attacker.stats.weapon.damage : null;
			int damage = attacker.stats.str + Dice.Roll(weaponDamage) + Dice.Roll("1d4-2");

			if (damage > 0) {
				// apply damage
				defender.UpdateHp(-damage);

				// display damage info
				string[] arr = new string[] { "painA", "painB", "painC", "painD" };
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.1f, Random.Range(0.6f, 1.8f));
				sfx.Play("Audio/Sfx/Combat/hitB", 0.5f, Random.Range(0.8f, 1.2f));
				defender.Speak("-" + damage, Color.red);

				// create blood
				grid.CreateBlood(defender.transform.position, damage, Color.red);
				
				// set isDead to true
				if (defender.stats.hp == 0) {
					return true;
				}
			}

		// parry or dodge
		} else {
			int r = Random.Range(0, 2);
			if (r == 1) {
				string[] arr = new string[] { "swordB", "swordC" };
				sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.15f, Random.Range(0.6f, 1.8f));
				defender.Speak("Parry", Color.white);
			} else {
				sfx.Play("Audio/Sfx/Combat/swishA", 0.1f, Random.Range(0.5f, 1.2f));
				defender.Speak("Dodge", Color.white);
			}
		}

		return false;
	}

	// =====================================================
	// Shoot
	// =====================================================

	public void Shoot (Creature target, float delay = 0) {
		me.StopMoving();

		Attack(target, delay);

		// create bullet
		grid.CreateBullet(me.transform.localPosition, target.transform.localPosition, me.speed, 8, Color.yellow);
	}


	public void ShootToBreak (Entity target) {
		me.StopMoving();

		// create bullet
		grid.CreateBullet(me.transform.localPosition, target.transform.localPosition, me.speed, 8, Color.yellow);

		// break container
		Container container = (Container)target;
		container.StartCoroutine(container.Open(me));
	}


	// =====================================================
	// Attack
	// =====================================================

	public void AttackToBreak (Entity target) {
		float delay = me.state == CreatureStates.Moving ? me.speed : 0;
		me.StartCoroutine(AttackAnimation(target, delay, 4));
	}


	public void Attack (Creature target, float delay = 0) {
		if (me.state == CreatureStates.Using) { return; }
		if (target.state == CreatureStates.Dying) { return; }

		me.state = CreatureStates.Attacking;

		me.StartCoroutine(AttackAnimation(target, delay, 3));

		target.combat.Defend(me, delay);
	}


	private IEnumerator AttackAnimation (Tile target, float delay = 0, float advanceDiv = 2) {
		yield return new WaitForSeconds(delay);
		if (target == null) { yield break; }
		
		float duration = me.speed * 0.5f;

		sfx.Play("Audio/Sfx/Combat/woosh", 0.4f, Random.Range(0.5f, 1.5f));

		// move towards target
		float t = 0;
		Vector3 startPos = me.transform.localPosition;
		Vector3 endPos = startPos + (target.transform.position - me.transform.position).normalized / advanceDiv;
		while (t <= 1) {
			t += Time.deltaTime / duration;
			me.transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		// move back to position
		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / duration;
			me.transform.localPosition = Vector3.Lerp(endPos, startPos, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		me.state = CreatureStates.Idle;
	}


	// =====================================================
	// Defend
	// =====================================================

	public void Defend (Creature attacker, float delay = 0) {
		me.StopMoving();

		me.state = CreatureStates.Defending;
		me.StartCoroutine(DefendAnimation(attacker, delay, 8));
	}


	private IEnumerator DefendAnimation (Creature attacker, float delay = 0, float advanceDiv = 8) {
		yield return new WaitForSeconds(delay);

		// wait for impact
		float duration = me.speed * 0.5f;
		yield return new WaitForSeconds(duration);

		// get combat positions
		Vector3 startPos = new Vector3(me.x, me.y, 0);
		Vector3 vec = (new Vector3(attacker.x, attacker.y, 0) - startPos).normalized / advanceDiv;
		Vector3 endPos = startPos - vec;

		// resolve combat outcome and apply combat sounds and effects

		bool isDead = ResolveCombatOutcome(attacker, me);
		if (isDead) {
			Die(attacker);
			yield break;
		}

		// move towards attacker
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime / duration * 0.5f;
			me.transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		// move back to position
		t = 0;
		while (t <= 1) {
			t += Time.deltaTime / (duration * 0.5f);
			me.transform.localPosition = Vector3.Lerp(endPos, startPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		me.state = CreatureStates.Idle;

		// emmit event once the defender has finished this action
		attacker.UpdateGameTurn();
	}

	// =====================================================
	// Death
	// =====================================================

	protected void Die (Creature attacker, float delay = 0) {
		me.StopMoving();

		me.state = CreatureStates.Dying;
		me.StartCoroutine(DeathAnimation(attacker, delay));

		// get a list of all items carried by the creature
		List<Item> allItems = new List<Item>();
		foreach (CreatureInventoryItem invItem in me.inventory.items) {
			allItems.Add(invItem.item);
		}

		// spawn all the items carried by the creature
		me.SpawnItemsFromInventory(allItems);
	}


	protected IEnumerator DeathAnimation (Creature attacker, float delay = 0) {
		string[] arr = new string[] { "painA", "painB", "painC", "painD" };
		sfx.Play("Audio/Sfx/Combat/" + arr[Random.Range(0, arr.Length)], 0.3f, Random.Range(0.6f, 1.8f));
		sfx.Play("Audio/Sfx/Combat/hitB", 0.6f, Random.Range(0.5f, 2.0f));

		grid.CreateBlood(me.transform.localPosition, 16, Color.red);

		grid.SetCreature(me.x, me.y, null);
		Destroy(me.gameObject);

		// update attacker xp
		attacker.UpdateXp(me.stats.xpValue);

		// if player died, emit gameover event
		if (me is Player) {
			me.GameOver();
		}
		
		yield break;
	}
}
