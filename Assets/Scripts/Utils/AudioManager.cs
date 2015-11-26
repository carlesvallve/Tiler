using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoSingleton <AudioManager> {

	public Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
	public float generalVolume = 1.0f;


	public void Play(string wav, float volume = 1.0f, bool loop = false) {
		Play(wav, volume, 1.0f, loop);
	}


	public void Play(string wav, float volume = 1.0f, float pitch = 1.0f, bool loop = false) {
		AudioClip clip = Resources.Load(wav) as AudioClip;

		// Log error if clip could not be loaded
		if (!clip) {
			Debug.LogError("Error while loading audio clip --> " + wav);
			return;
		}

		// if sound plays in a loop, escape if is already playing
		if (loop && audioSources.ContainsKey(wav)) {
				return;
		} 
				
		// Add the audio source component
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.loop = loop;

		// Log error if source could not be created
		if(!source) {
			print("Error while creating audio source component!");
			return;
		}

		// Set audio source props
		source.clip = clip;
		source.volume = volume * generalVolume;
		source.pitch = pitch;

		// Play it
		source.Play();

		if (loop)  {
			// if sound is playing in a loop, add it to the audiosources dictionary
			audioSources.Add(wav, source);
		} else {
			// otherwise, destroy the source component after the sound has played
			Destroy(source, clip.length);
		}
	}


	public void Stop(string wav) {
		if (audioSources.ContainsKey(wav)) {
			// Stop source and remove its component
			AudioSource source = audioSources[wav];
			source.Stop();
			Destroy(source);

			// Remove audio source from sources dictionary
			audioSources.Remove(wav);

		} else {

			// Log error if source could not be removed
			Debug.LogError("Error while stopping AudioSource --> " + wav);
		}
	}


	public bool IsPlaying (string wav) {
		// Note: only sounds playing in a loop are added to the dictionary
		return audioSources.ContainsKey(wav);
	}

}
