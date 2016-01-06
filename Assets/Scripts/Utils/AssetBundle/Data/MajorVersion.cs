using System;
using System.Collections.Generic;

namespace WizUtils.AssetBundles.Data {
	[Serializable]
	public class MajorVersion {
		public int majorversion;
		public List<string> versions;
		public List<Bundle> bundles;

		/// <summary>
		/// Initializes a new instance of the MajorVersion class.
		/// </summary>
		/// <param name="version">Version.</param>
		public MajorVersion(string version) {
			int majorVersion = 0;
			try {
				majorVersion = int.Parse(version.Split('.')[0]);
			} catch {
				Console.Write("Can't parse the major version");
			}

			this.majorversion = majorVersion;
			this.versions = new List<string>() { version };
			this.bundles = new List<Bundle>();
		}

		/// <summary>
		/// Adds the supported versions.
		/// </summary>
		/// <param name="version">Version.</param>
		public void AddSupportedVersions(string version) {
			if (versions.Contains(version)) {
				return;
			}

			versions.Add(version);
		}

		/// <summary>
		/// Gets a bundle by name
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="name">Name.</param>
		public Bundle GetByName(string name) {
			foreach (Bundle bundle in this.bundles) {
				if (bundle.name == name) {
					return bundle;
				}
			}

			return null;
		}
	}
}

