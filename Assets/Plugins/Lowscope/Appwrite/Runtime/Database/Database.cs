using System;
using System.Collections.Generic;
using Google.MiniJSON;
using System.Net;
using Lowscope.AppwritePlugin;
using Lowscope.AppwritePlugin.Accounts.Model;
using Cysharp.Threading.Tasks;
using Lowscope.AppwritePlugin.Accounts.Enums;
using Newtonsoft.Json.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;
using Newtonsoft.Json;
using Lowscope.AppwritePlugin.Utils;

namespace Lowscope.AppwritePlugin.Database
{
    public class Database : Service
    {
        internal Database(AppwriteConfig config) : base(config)
        {
        }

        public async UniTask<JObject> ListDocuments(string databaseId, string collectionId, List<string> query = null)
        {

            string url = $"{config.AppwriteURL}/databases/{{databaseId}}/collections/{{collectionId}}/documents"
                .Replace("{databaseId}", databaseId).Replace("{collectionId}", collectionId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, user?.Cookie);
            request.SetTimeout(30);
            var (json, httpStatusCode) = await request.Send();

            if (httpStatusCode == 0)
            {
                throw new AppwriteException(AppwriteException.Error.UnknownError, "Unrecognized response: " + json);
            }

            var jsonObj = JObject.Parse(json);
            if (httpStatusCode == HttpStatusCode.OK)
            {
                return jsonObj;
            }
            else
            {
                ProcessHttpStatusCode(httpStatusCode);

                return null;
            }
        }

        public void GetDocument(string databaseId, string collectionId, string documentId)
        {

        }
    }
}


