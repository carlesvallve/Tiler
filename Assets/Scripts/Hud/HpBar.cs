using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {

	private Creature creature;
	private SpriteRenderer percent;
	private SpriteRenderer shadow;

	
	public void Init (Creature creature) {
		this.creature = creature;

		percent = transform.Find("Percent").GetComponent<SpriteRenderer>();
		shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
		shadow.gameObject.SetActive(false);

		transform.localPosition = Vector3.zero + Vector3.up * 0.75f;
	}


	public void SetShadow (float value) {
		shadow.color = new Color(0, 0, 0, value);
		shadow.gameObject.SetActive(value > 0);
	}


	public void UpdateHp () {
		float value = (float)creature.stats.hp / creature.stats.hpMax;
		float x = -0.5f + (value / 2) + (1 - value) / 4;

		percent.transform.localPosition = new Vector3(x, percent.transform.localPosition.y, 0);
		percent.transform.localScale = new Vector3(value, percent.transform.localScale.y, 1);

		// set color
		if (value <= 0.25f) {
			percent.color = Color.red;
		} else if (value <= 0.5) {
			percent.color = Color.yellow;
		} else {
			percent.color = Color.green;
		}
	}
	
}
