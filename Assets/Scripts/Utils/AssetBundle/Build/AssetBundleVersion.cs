// .Net includes
using System;
using System.IO;

/// <summary>
/// Version build namespace
/// </summary>
namespace WizUtils.AssetBundles.Build{

	/// <summary>
	/// Version number is to update and set Version settings
	/// </summary>
	public static class AssetBundleVersion{

		/// <summary>
		/// Compares two Semantic Version strings, this uses the same type of int return as the IComparable
		/// interface, although it is not implemented here.
		/// TODO: Implement Semantic Version Number Class
		/// </summary>
		/// <returns>0 is they are the same, -1 if ver1 > ver2, 1 if ver2 > ver1</returns>
		/// <param name="ver1">Ver1.</param>
		/// <param name="ver2">Ver2.</param>
		public static int Compare(string ver1, string ver2) {

			// String comparison to speed up processing for duplicate submissions
			if (ver1 == ver2) {
				return 0;
			}

			// split the version number
			string[] ver1nums = ver1.Split('.');
			string[] ver2nums = ver2.Split('.');

			// find the max version len
			string[] longer = ver1nums.Length > ver2nums.Length ? ver1nums : ver2nums;
			int len = longer.Length;

			// itterate along and look for differences until we have found
			for (int i = 0; i < len; i++) {
				int ver1int = 0;
				int ver2int = 0;

				// Make sure we have both numbers to compare
				if (ver1nums.Length > i && ver2nums.Length > i) {

					if (TryParseVersionNumber(ver1nums[i], out ver1int) &&
						TryParseVersionNumber(ver2nums[i], out ver2int)) {

						if (ver1int == ver2int) {
							continue;
						}

						if (ver1int > ver2int) {
							return -1;
						}

						return 1;
					}

					throw new Exception("Parse Error");

				} else {
					// Check based on the greater length number being bigger than 0
					// For instance 1.2 vs 1.2.1 all semantic versioning should be limited
					// to 3 numbers, it can not be 1.2.0.1.

					int aNum = 0;

					if (!TryParseVersionNumber(longer[i], out aNum)) {
						throw new Exception("Parse Error");
					}

					if (aNum == 0) {
						return 0;
					}

					return ver1nums == longer ? -1 : 1;
				}
			}

			// If it gets through the loop comparing every number and it passes then they are the same.
			return 0;
		}

		/// <summary>
		/// Parses the major version.
		/// </summary>
		/// <returns>The major version.</returns>
		/// <param name="versionString">Version string.</param>
		public static int ParseMajorVersion(string versionString) {
			try {
				return int.Parse(versionString.Split('.') [0]);
			} catch {
				return 0;
			}
		}

		private static bool TryParseVersionNumber(string versionString, out int versionInt) {
			if (!int.TryParse(versionString, out versionInt)) {
				string[] patchnums = versionString.Split('-');

				if (!int.TryParse(patchnums[0], out versionInt)) {
					return false;
				}
			}

			return true;
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Updates the android version code.
		/// </summary>
		public static void UpdateAndroidVersionCode() {
			// Android version codes (APK version) should not have to be manually maintained. We translate our
			// bundle version into an integer (each number chunk increases the integer 100-fold) to define a
			// new version code. If the generated version code is higher than what it was before, we use it.

			string[] chunks = UnityEditor.PlayerSettings.bundleVersion.Split('.');
			int versionCode = 0;
			int code;

			for (int i = 0; i < chunks.Length; i += 1) {
				versionCode *= 100;
				int.TryParse(chunks[i], out code);
				versionCode += code;
			}

			if (versionCode > UnityEditor.PlayerSettings.Android.bundleVersionCode) {
				UnityEditor.PlayerSettings.Android.bundleVersionCode = versionCode;
			}
		}

		/// <summary>
		/// Bumps the version part.
		/// </summary>
		/// <param name="part">Part.</param>
		public static void BumpVersionPart(int part) {
			string[] chunks = UnityEditor.PlayerSettings.bundleVersion.Split('.');
			int num;

			if (part > chunks.Length - 1) {
				throw new Exception("This project currently only has " + chunks.Length + " parts in its version and will need at least 3");
			}

			int.TryParse(chunks[part], out num);
			chunks[part] = (num + 1).ToString();

			for (int i = part + 1; i < chunks.Length; i++) {
				chunks[i] = "0";
			}

			UnityEditor.PlayerSettings.bundleVersion = string.Join(".", chunks);

			UpdateAndroidVersionCode();
		}

		#endif

		/// <summary>
		/// This method will throw if this file is not there,
		/// this is mainly because it reflects the VersionNumber class
		/// which should be there before it gets here.  If it isnt here
		/// then we have an issue and the build should cancel
		/// </summary>
		/// <param name="file">File.</param>
		/// <param name="version">Version.</param>
		/// <param name="reset">If set to <c>true</c> reset.</param>
		public static void WriteBundleVersionToFile(string file, string version, bool reset) {
			if (file == null) {
				throw new NullReferenceException("Version File Location can not be null");
			}

			var fileContents = File.ReadAllText(file);

			fileContents = reset ? fileContents.Replace(version, "UNITY_BUNDLE_VERSION") : fileContents.Replace("UNITY_BUNDLE_VERSION", version);

			File.WriteAllText(file, fileContents);
		}
	}
}
