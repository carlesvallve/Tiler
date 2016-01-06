using UnityEngine;
using System;

namespace WizUtils.AssetBundles.Client{

	/// <summary>
	/// This class allow to load and instantiate easily any type of asset from an assetBundle
	/// </summary>
	public class AssetItem<T> : AssetItemBase where T : UnityEngine.Object {
		public delegate void AssetLoaded (T obj);

		public event AssetLoaded OnLoad;

		public delegate void AssetLoadFailed ();

		public event AssetLoadFailed OnFail;

		public AssetItem (string bundleName, string assetName) {
			this.bundleName = bundleName;
			this.assetName = assetName;
		}

		public override Type getType() {
			return typeof(T);
		}

		public override void Instantiate (AssetBundleLoadAssetOperation request) {
			T prefab = request.GetAsset<T> ();

			if (prefab != null) {
				T newInstance = GameObject.Instantiate (prefab) as T;
				newInstance.name = newInstance.name.Replace ("(Clone)", "");

				if (OnLoad != null) {
					OnLoad.Invoke (newInstance);
				}
			} else {
				if (OnFail != null) {
					OnFail.Invoke ();
				}
			}
		}
	}

}