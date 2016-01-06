using System;

namespace WizUtils.AssetBundles.Data {
	[Serializable]
	public class Bundle {
		public string name;
		public string url;
		public string hash;
		public string crc;

		public Bundle(string name = "", string url = "", string hash = "", string crc = "") {
			this.name = name;
			this.url = url;
			this.hash = hash;
			this.crc = crc;
		}
	}
}

