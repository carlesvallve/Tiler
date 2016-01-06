// .Net includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

// Unity includes
using UnityEngine;

// AssetsBundles
using WizUtils.AssetBundles.Common;
using WizUtils.AssetBundles.Data;

namespace WizUtils.AssetBundles.Client {

	public class AssetLoader : MonoSingleton<AssetLoader> {

		private List<AssetItemBase> assetList = new List<AssetItemBase>();

		// Global events
		public delegate void BeginLoading();
		public event BeginLoading OnBegin;

		public delegate void CompleteLoading();
		public event CompleteLoading OnComplete;

		public delegate void ErrorLoading(string error, string name);
		public event ErrorLoading OnError;

		public delegate void AssetLoading();
		public event AssetLoading OnAssetLoading;

		public delegate void ManifestLoaded();
		public event ManifestLoaded OnManifestLoaded;

		// Events
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String info) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		// Loading Progression
		private float progression = 0f;
		public float Progress { 
			get {
				return progression;
			}
			private set {
				progression = value;
				NotifyPropertyChanged("Progress");
			}
		}

		// Configuration
		// TODO: Get those values from the game settings
		private string urlBase = "http://wizcorp-zap-test.s3-website-ap-northeast-1.amazonaws.com/";
		private string manifestFile = "BundleVersions.json";
		private AssetManifest manifest = null;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start() {
			GameObject assetManager = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			assetManager.transform.SetParent(this.transform);
		}

		/// <summary>
		/// Load all the assets bundles required
		/// </summary>
		public void Load() {
			if (OnBegin != null) {
				OnBegin.Invoke();
			}

			StartCoroutine(LoadManifest());
		}

		/// <summary>
		/// Download all the assets bundles to have them in cache
		/// </summary>
		public void Download() {
			if (OnBegin != null) {
				OnBegin.Invoke();
			}

			StartCoroutine(LoadManifest(true));
		}

		/// <summary>
		/// Check that we have an updated version of every bundle.
		/// </summary>
		/// <returns>The bundles.</returns>
		public IEnumerator UpdateBundles() {

			MajorVersion mv = manifest.GetVersionSection(0);
			int max = mv.bundles.Count;
			int current = 0;

			foreach (Bundle bundle in mv.bundles) {
				string url = AssetBundleManager.BaseDownloadingURL + "/" + bundle.name;
				Hash128 hash = Hash128.Parse (bundle.hash);
				uint crc = uint.Parse (bundle.crc);

				// Load the asset
				WWW www = WWW.LoadFromCacheOrDownload(url, hash, crc);
				yield return www;

				if (!string.IsNullOrEmpty (www.error)) {
					if (OnError != null) {
						OnError.Invoke (www.error, bundle.name);
					}
					yield return null;
				} else {
					AssetBundle assetBundle = www.assetBundle;
					assetBundle.Unload(false);
				}
					
				current += 1;
				Progress = (float)current / (float)max * 100f;
			}
		}

		/// <summary>
		/// Loads the manifest.
		/// </summary>
		/// <returns>The manifest.</returns>
		protected IEnumerator LoadManifest(bool downloadBundles = false) {
			AssetBundleManager.SetAssetBundleServer(urlBase);
			manifest = AWSHelper.GetManifest(AssetBundleManager.BaseDownloadingURL, manifestFile);
			if (manifest == null) {
				throw new Exception("The manifest was not loaded");
			}

			if (downloadBundles) {
				yield return UpdateBundles ();
			}

			var request = AssetBundleManager.Initialize();
			if (request != null) {
				yield return StartCoroutine(request);
			}

			if (OnManifestLoaded != null) {
				OnManifestLoaded.Invoke();
			}

			StartCoroutine(LoadAssets());
		}

		/// <summary>
		/// Loads the assets.
		/// </summary>
		/// <returns>The assets.</returns>
		protected IEnumerator LoadAssets() {
			if (OnAssetLoading != null) {
				OnAssetLoading.Invoke();
			}

			foreach (AssetItemBase item in assetList) {
				yield return StartCoroutine(InstantiateGameObjectAsync(item));
			}

			// Clean asset bundle
			foreach (AssetItemBase item in assetList) {
				AssetBundleManager.UnloadAssetBundle(item.bundleName);
			}

			if (OnComplete != null) {
				OnComplete.Invoke();
			}
		}

		/// <summary>
		/// Instantiates the game object async.
		/// </summary>
		/// <returns>The game object async.</returns>
		/// <param name="item">Item.</param>
		public IEnumerator InstantiateGameObjectAsync(AssetItemBase item) {
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(item.bundleName, item.assetName, item.getType());
			if (request == null) {
				yield break;
			}
			yield return StartCoroutine(request);

			item.Instantiate(request);
		}

		/// <summary>
		/// Add an GameObject dependency, this asset will be loaded async (Load function)
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="onLoad">On load.</param>
		public void AddAsset(string bundleName, string assetName, AssetItem<GameObject>.AssetLoaded onLoad) {
			AssetItem<GameObject> item = new AssetItem<GameObject>(bundleName, assetName);
			item.OnLoad += onLoad;
			item.OnFail += () => {
				if (OnError != null) {
					OnError.Invoke("AssetLoadingError", assetName);
				}
			};
			assetList.Add(item);
		}

		/// <summary>
		/// Add an AudioClip dependency, this asset will be loaded async (Load function)
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="onLoad">On load.</param>
		public void AddAsset(string bundleName, string assetName, AssetItem<AudioClip>.AssetLoaded onLoad) {
			AssetItem<AudioClip> item = new AssetItem<AudioClip>(bundleName, assetName);
			item.OnLoad += onLoad;
			item.OnFail += () => {
				if (OnError != null) {
					OnError.Invoke("AssetLoadingError", assetName);
				}
			};
			assetList.Add(item);
		}

		/// <summary>
		/// Add an Sprite dependency, this asset will be loaded async (Load function)
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="onLoad">On load.</param>
		public void AddAsset(string bundleName, string assetName, AssetItem<Sprite>.AssetLoaded onLoad) {
			AssetItem<Sprite> item = new AssetItem<Sprite>(bundleName, assetName);
			item.OnLoad += onLoad;
			item.OnFail += () => {
				if (OnError != null) {
					OnError.Invoke("AssetLoadingError", assetName);
				}
			};
			assetList.Add(item);
		}
	}
}