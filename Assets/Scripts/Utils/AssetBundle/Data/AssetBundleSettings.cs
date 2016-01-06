using System;

/// <summary>
/// Asset bundle build namespace
/// </summary>
namespace WizUtils.AssetBundles.Data {

	/// <summary>
	/// Asset bundle settings.
	/// </summary>
	public class AssetBundleSettings {

		// Bundle information
		public string BundleVersion { get; private set; }
		public string BundleIdentifier { get; private set; }
		public string BundlesLocation { get; private set; }

		// Manifest information
		public bool LoadFromS3 { get; private set; }
		public string S3Url { get; private set; }
		public string S3ManifestName { get; private set; }
		// File location to read / write to
		public string ManifestLocation { get; private set; }

		/// <summary>
		/// Set up to configure the use of the AssetBundle class, removes needlessly
		/// long method signature and null checking
		/// </summary>
		/// <param name="bundleVersion">Build/Bundle Version of the Unity Game</param>
		/// <param name="bundleIdentifier">Build/Bundle Identifier</param>
		/// <param name="bundlesLocation">Bundles location</param>
		/// <param name="manifestLocation">Manifest location on local file system</param>
		/// <param name="loadFromS3">If we should check the S3 URL for the manifest</param>
		/// <param name="s3Url">S3 URL of the manifest</param>
		/// <param name="s3ManifestName">S3 manifest name</param>
		public AssetBundleSettings(
			string bundleVersion,
			string bundleIdentifier,
			string bundlesLocation,
			string manifestLocation,
			bool loadFromS3 = false,
			string s3Url = "",
			string s3ManifestName = ""
			) {
			BundleVersion = bundleVersion;
			BundleIdentifier = bundleIdentifier;
			BundlesLocation = bundlesLocation;

			ManifestLocation = manifestLocation;

			LoadFromS3 = loadFromS3;
			S3Url = s3Url;
			S3ManifestName = s3ManifestName;

			if (LoadFromS3 &&
				string.IsNullOrEmpty(s3Url) ||
				string.IsNullOrEmpty(S3ManifestName)) {
				throw new ArgumentException("If using S3, Url, Bucket and Manifest name can not be null or empty");
			}
		}
	}
}
