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
using System.Collections.Specialized;
using System.Web;

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

            if (query != null)
            {
                NameValueCollection queryParams = HttpUtility.ParseQueryString(String.Empty);
                foreach( string qValue in query)
                {
                    queryParams.Add("queries[]", qValue);
                }

                url += "?" + queryParams.ToString();
            }



            using var request = new WebRequest(EWebRequestType.GET, url, headers, userIdentity.GetUser()?.Cookie);
            request.SetTimeout(30);

            try
            {
                var json = await request.Send();

            
                var jsonObj = JObject.Parse(json);
                return jsonObj;
            } catch (JsonReaderException e)
            {
                throw new AppwriteException("Could not parse response as json", e);
            } catch (UnityWebRequestException e)
            {
                throw new AppwriteException("Request exception", e);
            }

            
        }

        public void GetDocument(string databaseId, string collectionId, string documentId)
        {

        }
    }
}


