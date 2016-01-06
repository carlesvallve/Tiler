// .Net includes
using System;
using System.IO;
using System.Net;

// WizUtils
using WizUtils.AssetBundles.Data;

/// <summary>
/// Asset bundle build namespace
/// </summary>
namespace WizUtils.AssetBundles.Common {

	/// <summary>
	/// AWS helper class
	/// </summary>
	public static class AWSHelper {

		/// <summary>
		/// Gets the manifest file.
		/// </summary>
		/// <returns>The manifest file.</returns>
		/// <param name="url">URL.</param>
		/// <param name="bucket">Bucket.</param>
		/// <param name="file">File.</param>
		public static string GetJsonFile(string url, string file) {
			string fullFile = url + "/" + file;

			// Initialize request instance
			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(fullFile);
			httpRequest.Method = WebRequestMethods.Http.Get;

			WebResponse response = null;

			// Get the response, syncronously
			try {
				response = httpRequest.GetResponse();
			} catch {
				return null;
			}

			using (Stream stream = response.GetResponseStream()) {
				using (StreamReader readStream = new StreamReader(stream)) {
					return readStream.ReadToEnd();
				}
			}
		}

		public static AssetManifest GetManifest(string url, string file) {
			string jsonFile = AWSHelper.GetJsonFile(url, file);
			return AssetManifest.Parse(jsonFile);
		}
	}
}
