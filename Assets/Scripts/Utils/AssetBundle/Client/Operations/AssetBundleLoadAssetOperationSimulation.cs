using UnityEngine;

namespace WizUtils.AssetBundles.Client {
	
	public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation {
		Object simulatedObject;

		public AssetBundleLoadAssetOperationSimulation(Object simulatedObject) {
			this.simulatedObject = simulatedObject;
		}

		public override T GetAsset<T>() {
			return simulatedObject as T;
		}

		public override bool Update() {
			return false;
		}

		public override bool IsDone() {
			return true;
		}
	}

}
