using UnityEngine;
using System.Collections;

public class Blood : MonoBehaviour {

	ParticleSystem particles;

	public void Init (Vector3 pos, int maxParticles, Color color) {
		transform.localPosition = pos + Vector3.up * 0.25f;

		print (color);
		
		particles = GetComponent<ParticleSystem>();
		particles.GetComponent<Renderer>().sortingLayerName = "Ui";
		particles.startColor = color;
		particles.Emit(maxParticles);
		Destroy(gameObject, 1f);
	}
}
