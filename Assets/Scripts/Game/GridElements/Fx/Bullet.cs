using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	ParticleSystem particles;

	public void Init (Vector3 startPos, Vector3 endPos, float duration, int maxParticles, Color color) {
		startPos += Vector3.up * 0.25f;

		particles = GetComponent<ParticleSystem>();
		particles.GetComponent<Renderer>().sortingLayerName = "Ui";
		particles.startColor = color;
		particles.Emit(maxParticles);
		
		StartCoroutine(Move(startPos, endPos, duration));
	}

	private IEnumerator Move (Vector3 startPos, Vector3 endPos, float duration) {
		// interpolate bullet position
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

			yield return null;
		}

		Destroy(gameObject, duration);
	}
}
