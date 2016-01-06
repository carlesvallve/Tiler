using UnityEngine;

namespace WizUtils.AssetBundles.Client{
	
	public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull {

		public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type) : base (bundleName, assetName, type) {
		}

		public override bool Update() {
			base.Update ();

			if (request != null && request.isDone) {
				AssetBundleManager.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
				return false;
			}
			return true;
		}
	}

}
