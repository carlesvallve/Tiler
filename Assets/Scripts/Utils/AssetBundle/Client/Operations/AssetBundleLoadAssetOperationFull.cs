using UnityEngine;

namespace WizUtils.AssetBundles.Client{
	
	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation {
		protected string assetBundleName;
		protected string assetName;
		protected string downloadingError;
		protected System.Type type;
		protected AssetBundleRequest request = null;

		public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type) {
			this.assetBundleName = bundleName;
			this.assetName = assetName;
			this.type = type;
		}

		public override T GetAsset<T>() {
			if (request != null && request.isDone) {
				return request.asset as T;
			}
			return null;
		}

		// Returns true if more Update calls are required.
		public override bool Update() {
			if (request != null) {
				return false;
			}

			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out downloadingError);
			if (bundle != null) {
				///@TODO: When asset bundle download fails this throws an exception...
				try {
					request = bundle.m_AssetBundle.LoadAssetAsync(assetName, type);
				} catch (System.Exception ex) {
					Debug.Log(ex);
				}

				return false;
			} else {
				return true;
			}
		}

		public override bool IsDone() {
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (request == null && downloadingError != null) {
				Debug.LogError(downloadingError);
				return true;
			}

			return request != null && request.isDone;
		}
	}

}
