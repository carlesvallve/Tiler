using UnityEngine;
using System.Collections;

public class Blood : MonoBehaviour {

	ParticleSystem particles;

	public void Init (Vector3 pos, int maxParticles) {
		transform.localPosition = pos + Vector3.up * 0.25f;

		particles = GetComponent<ParticleSystem>();
		particles.GetComponent<Renderer>().sortingLayerName = "Ui";
		particles.Emit(maxParticles);
		Destroy(gameObject, 1f);
	}
}
