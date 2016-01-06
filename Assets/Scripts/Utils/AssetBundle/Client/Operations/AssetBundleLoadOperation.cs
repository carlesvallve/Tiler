﻿using System.Collections;

namespace WizUtils.AssetBundles.Client {
	
	public abstract class AssetBundleLoadOperation : IEnumerator {
		public object Current {
			get {
				return null;
			}
		}

		public bool MoveNext() {
			return !IsDone();
		}

		public void Reset () {
		}

		public abstract bool Update ();

		public abstract bool IsDone ();
	}

}