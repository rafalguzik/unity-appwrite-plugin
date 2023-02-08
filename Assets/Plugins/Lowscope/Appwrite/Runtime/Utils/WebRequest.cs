using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Lowscope.AppwritePlugin.Utils
{
	public class WebRequest : IDisposable
	{
		private readonly UnityWebRequest webRequest;
		private readonly EWebRequestType requestType;

		public WebRequest(EWebRequestType requestType, string url, Dictionary<string,string> headers, string cookie,
			byte[] data = null)
		{
			UnityWebRequest.ClearCookieCache(new Uri(url[..url.IndexOf("v1", StringComparison.InvariantCulture)]));

			this.requestType = requestType;

			switch (requestType)
			{
				case EWebRequestType.GET:
					webRequest = UnityWebRequest.Get(url);
					break;
				case EWebRequestType.POST:
					// Workaround to send byte[] data over a post request. Instead of text.
					webRequest = UnityWebRequest.Put(url, data);
					webRequest.method = "POST";
					break;
				case EWebRequestType.PUT:
					webRequest = UnityWebRequest.Put(url, data);
					break;
				case EWebRequestType.PATCH:
					webRequest = new UnityWebRequest(url, "PATCH", new DownloadHandlerBuffer(), new UploadHandlerRaw(data));
					break;
				case EWebRequestType.DELETE:
					webRequest = UnityWebRequest.Delete(url);
					break;
				default:
					webRequest = null;
					break;
			}

			foreach (var (key, value) in headers)
				webRequest?.SetRequestHeader(key, value);

			if (!string.IsNullOrEmpty(cookie))
				webRequest?.SetRequestHeader("Cookie", cookie);
		}

		public async UniTask<string> Send()
		{
			try
			{
				await webRequest.SendWebRequest();
				var responseCode = (int)webRequest.responseCode;

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    throw new AppwriteException("Encountered an error during Send operation: " + webRequest.result + " code=" + webRequest.responseCode, webRequest.responseCode);
                }

                if (requestType == EWebRequestType.DELETE)
					return "";
						
				string text = webRequest?.downloadHandler.text;
				return text;

			}
			catch (UnityWebRequestException exception)
			{
				throw new AppwriteException("Encountered an error during Send operation: " + exception.Message, exception);
            }
		}

		public async UniTask<bool> Download(string path, Action<ulong> progressCallback)
		{
            try
            {
				DownloadHandlerFile dh = new DownloadHandlerFile(path);
				webRequest.downloadHandler = dh;
                UnityWebRequestAsyncOperation downloadOperation = webRequest.SendWebRequest();

				var headers = webRequest.GetResponseHeaders();

				while (!downloadOperation.isDone)
				{
					progressCallback?.Invoke(webRequest.downloadedBytes);
					await UniTask.Delay(100);
				}

				if (webRequest.result != UnityWebRequest.Result.Success)
				{
					throw new AppwriteException("Encountered an error during Download operation: " + webRequest.result + " code=" + webRequest.responseCode, webRequest.responseCode);
				}

                var responseCode = (int)webRequest.responseCode;

				return true;

            }
            catch (UnityWebRequestException exception)
            {
				throw new AppwriteException("Encountered an error during Download operation", exception);
            }
        }

		public void Dispose()
		{
			webRequest?.Dispose();
		}

		public string ExtractCookie()
		{
			return webRequest.GetResponseHeaders().TryGetValue("Set-Cookie", out string cookie)
				? cookie[..cookie.IndexOf(" expires=", StringComparison.InvariantCulture)]
				: "";
		}

		public float GetDownloadProgress()
		{
			return webRequest.downloadProgress;
		}

		public void SetTimeout(int timeout)
		{
			webRequest.timeout = timeout;
		}
	}
}