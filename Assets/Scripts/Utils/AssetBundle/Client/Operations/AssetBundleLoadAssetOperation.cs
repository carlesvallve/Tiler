namespace WizUtils.AssetBundles.Client {
	
	public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation {
		public abstract T GetAsset<T>() where T : UnityEngine.Object;
	}

}
