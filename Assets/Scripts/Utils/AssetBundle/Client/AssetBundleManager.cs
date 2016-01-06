// .Net includes
using System.Collections.Generic;

// AssetsBundles
using WizUtils.AssetBundles.Common;

// Unity includes
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WizUtils.AssetBundles.Client{

	public class AssetBundleManager : MonoBehaviour {
		
		private static string baseDownloadingURL = "";
		private static AssetBundleManifest assetBundleManifest = null;

		private static Dictionary<string, LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
		private static Dictionary<string, WWW> downloadingWWWs = new Dictionary<string, WWW>();
		private static Dictionary<string, string> downloadingErrors = new Dictionary<string, string>();
		private static List<AssetBundleLoadOperation> inProgressOperations = new List<AssetBundleLoadOperation>();
		private static Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();

		/// <summary>
		/// The base downloading url which is used to generate the full downloading url with the assetBundle names
		/// </summary>
		/// <value>The base downloading UR.</value>
		public static string BaseDownloadingURL {
			get { return baseDownloadingURL; }
			set { baseDownloadingURL = value; }
		}

		/// <summary>
		/// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		/// </summary>
		/// <value>The asset bundle manifest object.</value>
		public static AssetBundleManifest AssetBundleManifestObject {
			set { assetBundleManifest = value; }
		}

		#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		static int simulateAssetBundleInEditor = -1;
		static int localAssetBundleInEditor = -1;

		// Simulation Mode (don't use bundles)
		public static bool SimulateAssetBundleInEditor {
			get {
				if (simulateAssetBundleInEditor == -1) {
					simulateAssetBundleInEditor = EditorPrefs.GetBool ("SimulateAssetBundles", true) ? 1 : 0;
				}

				return simulateAssetBundleInEditor != 0;
			}
			set {
				int newValue = value ? 1 : 0;
				if (newValue != simulateAssetBundleInEditor) {
					simulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool ("SimulateAssetBundles", value);
				}
			}
		}

		// Local Server Mode (download asset bundles directly on localhost)
		public static bool LocalAssetBundleInEditor {
			get {
				if (localAssetBundleInEditor == -1) {
					localAssetBundleInEditor = EditorPrefs.GetBool ("LocalAssetBundles", true) ? 1 : 0;
				}

				return localAssetBundleInEditor != 0;
			}
			set {
				int newValue = value ? 1 : 0;
				if (newValue != localAssetBundleInEditor) {
					localAssetBundleInEditor = newValue;
					EditorPrefs.SetBool ("LocalAssetBundles", value);
				}
			}
		}
		#else
		public static bool SimulateAssetBundleInEditor {
			get {return false;}
			set {}
		}

		public static bool LocalAssetBundleInEditor {
			get {return false;}
			set {}
		}
		#endif

		/// <summary>
		/// Sets the asset bundle server url.
		/// This is the base url used to download manifest & bundles
		/// </summary>
		/// <param name="url">URL.</param>
		public static void SetAssetBundleServer(string url) {
			if (SimulateAssetBundleInEditor) {
				BaseDownloadingURL = url;
				return;
			}

			if (LocalAssetBundleInEditor) {
				url = "http://localhost:7888/";
			}

			BaseDownloadingURL = url + Utility.GetPlatformName() + "/";
		}

		/// <summary>
		/// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		/// </summary>
		/// <returns>The loaded asset bundle.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="error">Error.</param>
		public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error){
			if (downloadingErrors.TryGetValue(assetBundleName, out error)) {
				return null;
			}

			LoadedAssetBundle bundle = null;
			loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null) {
				return null;
			}

			// No dependencies are recorded, only the bundle itself is required.
			string[] bundleDependencies = null;
			if (!dependencies.TryGetValue(assetBundleName, out bundleDependencies)) {
				return bundle;
			}

			// Make sure all dependencies are loaded
			foreach (string dependency in bundleDependencies) {
				if (downloadingErrors.TryGetValue(assetBundleName, out error)) {
					return bundle;
				}

				LoadedAssetBundle dependentBundle;
				loadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null) {
					return null;
				}
			}

			return bundle;
		}

		/// <summary>
		/// Initialize an instance of the asset bundler manager
		/// This function download the root manifest for the current platform
		/// </summary>
		public static AssetBundleLoadManifestOperation Initialize() {
			return Initialize(Utility.GetPlatformName());
		}
			
		/// <summary>
		/// Initialize the specified manifestAssetBundleName.
		/// </summary>
		/// <param name="manifestAssetBundleName">Manifest asset bundle name.</param>
		private static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName) {
			if (SimulateAssetBundleInEditor) {
				return null;
			}

			LoadAssetBundle(manifestAssetBundleName, true);
			var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			inProgressOperations.Add(operation);
			return operation;
		}

		/// <summary>
		/// Load asset from the given assetBundle.
		/// </summary>
		/// <returns>The asset async.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="assetName">Asset name.</param>
		/// <param name="type">Type.</param>
		public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type) {
			AssetBundleLoadAssetOperation operation = null;
			if (SimulateAssetBundleInEditor) {
				#if UNITY_EDITOR
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0) {
					Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}

				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths [0]);
				operation = new AssetBundleLoadAssetOperationSimulation(target);
				#endif
			} else {
				LoadAssetBundle(assetBundleName);
				operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

				inProgressOperations.Add(operation);
			}

			return operation;
		}

		/// <summary>
		/// Unload assetbundle and its dependencies.
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		public static void UnloadAssetBundle(string assetBundleName) {
			if (SimulateAssetBundleInEditor) {
				return;
			}

			UnloadAssetBundleInternal(assetBundleName);
			UnloadDependencies(assetBundleName);
		}

		/// <summary>
		/// Load AssetBundle and its dependencies
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="isLoadingAssetBundleManifest">If set to true is loading asset bundle manifest.</param>
		protected static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false) {
			if (SimulateAssetBundleInEditor) {
				return;
			}

			if (!isLoadingAssetBundleManifest) {
				if (assetBundleManifest == null) {
					Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
					return;
				}
			}

			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

			// Load dependencies.
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest) {
				LoadDependencies(assetBundleName);
			}
		}

		/// <summary>
		/// Where we actuall call WWW to download the assetBundle.
		/// </summary>
		/// <returns>true, if asset bundle internal was loaded, false otherwise.</returns>
		/// <param name="assetBundleName">Asset bundle name.</param>
		/// <param name="isLoadingAssetBundleManifest">If set to true is loading asset bundle manifest.</param>
		protected static bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest) {
			LoadedAssetBundle bundle = null;
			loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null) {
				bundle.m_ReferencedCount++;
				return true;
			}

			if (downloadingWWWs.ContainsKey(assetBundleName)) {
				return true;
			}

			WWW download = null;
			string url = baseDownloadingURL + assetBundleName;

			// For manifest assetbundle, always download it as we don't have hash for it.
			if (isLoadingAssetBundleManifest) {
				download = WWW.LoadFromCacheOrDownload(url, 0);
			} else {
				download = WWW.LoadFromCacheOrDownload(url, assetBundleManifest.GetAssetBundleHash(assetBundleName), 0);
			}

			downloadingWWWs.Add(assetBundleName, download);
			return false;
		}

		/// <summary>
		/// Where we get all the dependencies and load them all.
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		protected static void LoadDependencies(string assetBundleName) {
			if (assetBundleManifest == null) {
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}

			// Get dependecies from the AssetBundleManifest object.
			string[] bundleDependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
			if (bundleDependencies.Length == 0) {
				return;
			}

			// Record and load all dependencies.
			dependencies.Add(assetBundleName, bundleDependencies);
			for (int i = 0; i < bundleDependencies.Length; i++) {
				LoadAssetBundleInternal(bundleDependencies[i], false);
			}
		}

		/// <summary>
		/// Unload all the depencies used by one bundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		protected static void UnloadDependencies(string assetBundleName) {
			string[] bundleDependencies = null;
			if (!dependencies.TryGetValue(assetBundleName, out bundleDependencies)) {
				return;
			}

			foreach (string dependency in bundleDependencies) {
				UnloadAssetBundleInternal(dependency);
			}

			dependencies.Remove(assetBundleName);
		}

		/// <summary>
		/// Unload a bundle
		/// </summary>
		/// <param name="assetBundleName">Asset bundle name.</param>
		protected static void UnloadAssetBundleInternal(string assetBundleName) {
			string error;
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null) {
				return;
			}

			if (--bundle.m_ReferencedCount == 0) {
				bundle.m_AssetBundle.Unload(false);
				loadedAssetBundles.Remove(assetBundleName);
			}
		}

		/// <summary>
		/// Collect all the finished WWWs
		/// </summary>
		void Update() {
			var keysToRemove = new List<string>();
			foreach (var keyValue in downloadingWWWs) {
				WWW download = keyValue.Value;

				// If downloading fails.
				if (download.error != null) {
					downloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
					keysToRemove.Add(keyValue.Key);
					continue;
				}

				// If downloading succeeds.
				if (download.isDone) {
					AssetBundle bundle = download.assetBundle;
					if (bundle == null) {
						downloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
						keysToRemove.Add(keyValue.Key);
						continue;
					}

					loadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle));
					keysToRemove.Add(keyValue.Key);
				}
			}

			// Remove the finished WWWs.
			foreach (var key in keysToRemove) {
				WWW download = downloadingWWWs [key];
				downloadingWWWs.Remove(key);
				download.Dispose();
			}

			// Update all in progress operations
			for (int i = 0; i < inProgressOperations.Count;) {
				if (!inProgressOperations [i].Update()) {
					inProgressOperations.RemoveAt(i);
				} else {
					i++;
				}
			}
		}
	}
}