using System;
using System.Collections.Generic;
using Google.MiniJSON;
using System.Net;
using Lowscope.AppwritePlugin;
using Lowscope.AppwritePlugin.Accounts.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;
using Newtonsoft.Json;
using Lowscope.AppwritePlugin.Utils;
using Lowscope.AppwritePlugin.Identity;

namespace Lowscope.AppwritePlugin.Database
{
    public class Database : Service
    {
        internal Database(AppwriteConfig config, IUserIdentity userIdentity) : base(config, userIdentity)
        {
        }

        public async UniTask<JObject> ListDocuments(string databaseId, string collectionId, List<string> query = null)
        {

            string url = $"{config.AppwriteURL}/databases/{{databaseId}}/collections/{{collectionId}}/documents"
                .Replace("{databaseId}", databaseId).Replace("{collectionId}", collectionId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, userIdentity.GetUser()?.Cookie);
            request.SetTimeout(30);
            var json = await request.Send();

            var jsonObj = JObject.Parse(json);

            return jsonObj;
        }

        public void GetDocument(string databaseId, string collectionId, string documentId)
        {

        }
    }
}


