using System;
using System.Net;
using Cysharp.Threading.Tasks;
using Lowscope.AppwritePlugin.Accounts.Enums;
using Lowscope.AppwritePlugin.Database;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;

namespace Lowscope.AppwritePlugin.Storage
{
	public class Storage: Service
	{
		public Storage(AppwriteConfig config): base(config)
		{
		}

		public async UniTask<byte[]> GetFileDownload(string bucketId,string fileId)
		{
            if (user == null)
            {
                //todo: throw exception
            }

            string url = $"{config.AppwriteURL}/storage/buckets/{{bucketId}}/files/{{fileId}}/download"
                .Replace("{bucketId}", bucketId).Replace("{fileId}", fileId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, user.Cookie);
            var (data, httpStatusCode) = await request.Download();

            if (httpStatusCode == HttpStatusCode.OK)
            {
                return data;
            }
            else
            {
                switch (httpStatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                    //todo: throw unautorized exception
                    case HttpStatusCode.NotFound:
                        return null;
                }

                return null;
            }
        }
    }
}

