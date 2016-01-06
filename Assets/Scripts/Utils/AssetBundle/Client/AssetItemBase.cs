using UnityEngine;
using System;

namespace WizUtils.AssetBundles.Client{

	public abstract class AssetItemBase{
		public string bundleName;
		public string assetName;

		public virtual Type getType (){
			return typeof(GameObject);
		}

		public virtual void Instantiate (AssetBundleLoadAssetOperation request){
		}

		public string toString (){
			return "[" + bundleName + " -> " + assetName + "]";
		}
	}

}