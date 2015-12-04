using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {

	private Creature creature;
	private int maxHp;

	private SpriteRenderer percent;
	private SpriteRenderer shadow;

	
	public void Init (Creature creature) {
		this.creature = creature;

		transform.localPosition = Vector3.zero + Vector3.up * 0.75f;

		/*SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) {
        	sprite.sortingLayerName = "Ui";
        }*/

		percent = transform.Find("Percent").GetComponent<SpriteRenderer>();
		shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
		shadow.gameObject.SetActive(false);
	}


	public void UpdateHp (int hp) {
		float value = (float)hp / creature.maxHp;

		float x = -0.5f + (value / 2) + (1-value) / 4;
		percent.transform.localPosition = new Vector3(x, percent.transform.localPosition.y, 0);
		percent.transform.localScale = new Vector3(value, percent.transform.localScale.y, 1);


	}

}
