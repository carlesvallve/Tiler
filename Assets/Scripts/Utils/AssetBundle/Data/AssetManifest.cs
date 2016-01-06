// .Net includes
using System;
using System.Collections.Generic;

// Unity includes
using UnityEngine;

namespace WizUtils.AssetBundles.Data {
	[Serializable]
	public class AssetManifest {
		public string application;
		public string version;
		public List<MajorVersion> assetbundles;

		public AssetManifest(string bundleIdentifier, string version) {
			this.application = bundleIdentifier;
			this.version = version;
			this.assetbundles = new List<MajorVersion>();
		}

		/// <summary>
		/// Return a json representation of the current manifest
		/// </summary>
		/// <returns>The json.</returns>
		public string ToJson() {
			return JsonUtility.ToJson(this);
		}

		/// <summary>
		/// Parse the specified json and create a Manifest Object
		/// </summary>
		/// <param name="json">Json.</param>
		public static AssetManifest Parse(string json) {
			return JsonUtility.FromJson<AssetManifest>(json);
		}

		/// <summary>
		/// Function to get a majorversion directly by version number
		/// </summary>
		/// <returns>The version section.</returns>
		/// <param name="version">Version.</param>
		public MajorVersion GetVersionSection(int version) {
			if (this.assetbundles == null) {
				throw new NullReferenceException("Asset Bundle section can not be null");
			}

			foreach (MajorVersion majorSection in this.assetbundles) {
				if (majorSection.majorversion == version) {
					return majorSection;
				}
			}

			return null;
		}

	}
}

