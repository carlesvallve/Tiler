// .Net includes
using System;
using System.IO;

// WizUtils
using WizUtils.AssetBundles.Common;
using WizUtils.AssetBundles.Data;

/// <summary>
/// Build namespace for asset bundles
/// </summary>
namespace WizUtils.AssetBundles.Build {

	/// <summary>
	/// Asset bundle build helper, this aids the assetbundle process
	/// </summary>
	public class ManifestBuilder {

		/// <summary>
		/// Builds the asset manifest.
		/// </summary>
		/// <param name="bundleIdentifier">Bundle identifier.</param>
		/// <param name="bundleVersion">Bundle version.</param>
		/// <param name="bundleNames">Bundle names.</param>
		public static void BuildAssetManifest(AssetBundleSettings settings, string[] bundleNames) {

			// File locations
			string manifestLocation = settings.ManifestLocation;
			string bundlesLocation = settings.BundlesLocation;

			// Find the previous file and read it to get context, if no file is found generate a new file
			string jsonFile = AWSHelper.GetJsonFile(settings.S3Url, settings.S3ManifestName);
			string oldVersion = string.Empty;
			string currentVersion = settings.BundleVersion;

			AssetManifest manifest = null;

			// Create the json file
			if (jsonFile == null) {
				manifest = new AssetManifest(settings.BundleIdentifier, settings.BundleVersion);
			} else {
				manifest = AssetManifest.Parse(jsonFile);

				// If there is a file append the information to the file depending on if the major version has been updated
				oldVersion = manifest.version;

				if (AssetBundleVersion.Compare(oldVersion, currentVersion) == 0) {
					throw new Exception("Versions are the same please bump version number");
				}

				if (AssetBundleVersion.Compare(oldVersion, currentVersion) == -1) {
					throw new Exception("Old version is greater than the current version ...");
				}

				// Versions are different so we need to update bundle version
				manifest.version = currentVersion;
			}

			// Get major version
			int majorVersion = AssetBundleVersion.ParseMajorVersion(currentVersion);

			// Get the version section for this version
			MajorVersion mv = manifest.GetVersionSection(majorVersion);

			// If it is null create it
			if (mv == null) {
				mv = new MajorVersion(currentVersion);
				manifest.assetbundles.Add(mv);
			}

			// Add supported version to major section
			mv.AddSupportedVersions(currentVersion);

			foreach (var name in bundleNames) {
				Bundle oldBundle = mv.GetByName(name);
				if (oldBundle != null) {
					mv.bundles.Remove(oldBundle);
				}

				Bundle newBundle = CreateBundleSectionFromFile(majorVersion, bundlesLocation, name);
				mv.bundles.Add(newBundle);
			}

			WriteManifest(manifestLocation, manifest);
		}

		/// <summary>
		/// Creates the bundle section from file.
		/// </summary>
		/// <returns>The bundle section from file.</returns>
		/// <param name="majorVersion">Major version.</param>
		/// <param name="bundleLocation">Bundle location.</param>
		/// <param name="bundleName">Bundle name.</param>
		private static Bundle CreateBundleSectionFromFile(int majorVersion, string bundleLocation, string bundleName) {
			string hash = string.Empty;
			string crc = string.Empty;
			string url = "/" + majorVersion + "/" + bundleName;

			using (StreamReader sr = ReadBundleManifest(bundleLocation + "/" + bundleName + ".manifest")) {
				while (sr.Peek() != -1) {
					string line = sr.ReadLine();
					if (line.Contains("CRC")) {
						crc = line.Split(':')[1].Trim();
					}

					if (line.Contains("Hash:")) {
						hash = line.Split(':')[1].Trim();
					}

					if (crc != string.Empty && hash != string.Empty) {
						break;
					}
				}
			}
			return new Bundle(bundleName, url, hash, crc);
		}

		/// <summary>
		/// Reads the manifest for a specific bundle
		/// </summary>
		/// <returns>Stream to the manifest</returns>
		/// <param name="file">file location</param>
		private static StreamReader ReadBundleManifest(string file) {
			FileStream stream = null;

			try {
				stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
			} catch(FileNotFoundException fileNotFoundException) {
				//TODO: Add a logger
				Console.WriteLine(fileNotFoundException);
				return null;
			} catch(Exception error) {
				throw error;
			}

			// Stream reader
			return new StreamReader(stream);
		}

		/// <summary>
		/// Write the Json to the manifest file
		/// </summary>
		/// <param name="file">Full path and name of the file</param>
		/// <param name="json">Json to write</param>
		private static void WriteManifest(string file, AssetManifest json) {
			string path = Path.GetDirectoryName(file);
			if (!Directory.Exists(path)) {
				throw new Exception("No directory");
			}

			FileStream stream = null;

			try {
				stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, true);
			} catch {
				throw;
			}

			// Stream reader
			using (StreamWriter sw = new StreamWriter(stream)) {
				sw.Write(json.ToJson());
			}
		}
	}
}