using System;
using System.Net;
using System.Net.Mail;
using Cysharp.Threading.Tasks;
using Google.MiniJSON;
using Lowscope.AppwritePlugin.Accounts.Enums;
using Lowscope.AppwritePlugin.Storage.Model;
using Lowscope.AppwritePlugin.Utils;
using Newtonsoft.Json.Linq;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;

namespace Lowscope.AppwritePlugin.Storage
{
	public class Storage: Service
	{
		public Storage(AppwriteConfig config): base(config)
		{
		}

        public async UniTask<FileInfo> GetFile(string bucketId, string fileId)
        {
            string url = $"{config.AppwriteURL}/storage/buckets/{{bucketId}}/files/{{fileId}}"
                .Replace("{bucketId}", bucketId).Replace("{fileId}", fileId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, user?.Cookie);
            request.SetTimeout(30);
            var(json, httpStatusCode) = await request.Send();

            if (httpStatusCode == HttpStatusCode.OK)
            {
                var jsonObj = JObject.Parse(json);
                FileInfo file = new FileInfo()
                {
                    Id = (string)jsonObj.GetValue("$id"),
                    BucketId = (string)jsonObj.GetValue("bucketId"),
                    CreatedAt = (string)jsonObj.GetValue("$createdAt"),
                    UpdatedAt = (string)jsonObj.GetValue("$updatedAt"),
                    Permissions = jsonObj.Value<JArray>("$permissions").ToObject<string[]>(),
                    Name = (string)jsonObj.GetValue("name"),
                    Signature = (string)jsonObj.GetValue("signature"),
                    MimeType = (string)jsonObj.GetValue("mimeType"),
                    SizeTotal = (ulong)jsonObj.GetValue("sizeOriginal"),
                    ChunksTotal = (ulong)jsonObj.GetValue("chunksTotal"),
                    ChunksUploaded = (ulong)jsonObj.GetValue("chunksUploaded")

                };

                return file;
            } else
            {
                ProcessHttpStatusCode(httpStatusCode);
                return null;
            }
        }

		public async UniTask<bool> GetFileDownload(string bucketId,string fileId, string path, Action<ulong> progressCallback)
        { 

            string url = $"{config.AppwriteURL}/storage/buckets/{{bucketId}}/files/{{fileId}}/download"
                .Replace("{bucketId}", bucketId).Replace("{fileId}", fileId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, user?.Cookie);
            var httpStatusCode = await request.Download(path, progressCallback);

            if (httpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new AppwriteException(AppwriteException.Error.NotAuthorized, "User " + user.Id + "is not authorized to access file " + fileId + " in bucket " + bucketId);
                } else
                {
                    ProcessHttpStatusCode(httpStatusCode);
                }

                return false;
            }
        }
    }
}

