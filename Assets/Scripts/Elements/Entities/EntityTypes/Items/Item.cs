using UnityEngine;
using System.Collections;

public class Item : Entity {

	public ItemTypes type { get; set; }

	protected  bool picked = false;
	protected GameObject sprite;
	protected ParticleSystem particles;


	public virtual void Awake () {
		sprite = transform.Find("Sprite").gameObject;
		particles = transform.Find("Particles").GetComponent<ParticleSystem>();
	}


	public virtual void Pickup () {
		picked = true;
		sprite.SetActive(false);
		particles.Play();
		Destroy(gameObject, 1f);
	}


	public virtual bool HasBeenPickedUp () {
		return picked;
	}
}
