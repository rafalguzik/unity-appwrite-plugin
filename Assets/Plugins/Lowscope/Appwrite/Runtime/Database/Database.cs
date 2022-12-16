using System;
using System.Collections.Generic;
using Google.MiniJSON;
using System.Net;
using Lowscope.AppwritePlugin;
using Lowscope.AppwritePlugin.Accounts.Model;
using UnityEditor.PackageManager.Requests;
using Cysharp.Threading.Tasks;
using Lowscope.AppwritePlugin.Accounts.Enums;
using Newtonsoft.Json.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;
using Newtonsoft.Json;

namespace Lowscope.AppwritePlugin.Database
{
    public class Database : Service
    {
        internal Database(AppwriteConfig config) : base(config)
        {
        }

        public async UniTask<JObject> ListDocuments(string databaseId, string collectionId, List<string> query = null)
        {
            if (user == null)
            {
                //todo: throw exception
            }  

            string url = $"{config.AppwriteURL}/databases/{{databaseId}}/collections/{{collectionId}}/documents"
                .Replace("{databaseId}", databaseId).Replace("{collectionId}", collectionId);

            using var request = new WebRequest(EWebRequestType.GET, url, headers, user.Cookie);
            var (json, httpStatusCode) = await request.Send();

            if (httpStatusCode == HttpStatusCode.OK)
            {
                return JObject.Parse(json);
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

        public void GetDocument(string databaseId, string collectionId, string documentId)
        {

        }
    }
}


