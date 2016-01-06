using UnityEngine;
using System.Collections;

using WizUtils.AssetBundles.Client;


public class Assets : MonoBehaviour {

	public static void SetAssets () {
		// Get the singleton instance
		AssetLoader assetLoader = AssetLoader.instance;

		// Register a list of assets
		assetLoader.AddAsset("assets/views/home", "Background", (GameObject obj) => {
			//obj.transform.SetParent(transform, false);
		});

		assetLoader.AddAsset("assets/common/audio", "Menu_BGM_loop", (AudioClip audio) => {
			//BGM_loop = audio;
		});


		// Global Events
		assetLoader.OnBegin += () => {
			//gameContainer.ShowLoading(true);
		};

		assetLoader.OnError += (error, name) => {
			//Debug.Log(name + " - " + error);
		};

		assetLoader.OnComplete += () => {
			//Init();
			//gameContainer.ShowLoading(false);
		};

		// Start loading the assets
		assetLoader.Load();
	}
}
