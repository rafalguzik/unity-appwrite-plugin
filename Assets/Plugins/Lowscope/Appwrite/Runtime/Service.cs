using System;
using System.Collections.Generic;
using System.IO;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Utils;
using UnityEngine;
using System.Net;
using Lowscope.AppwritePlugin.Identity;
using System.Collections.Specialized;
using System.Text;

namespace Lowscope.AppwritePlugin
{
    public abstract class Service
    {
        protected AppwriteConfig config;
        protected readonly Dictionary<string, string> headers;

        protected IUserIdentity userIdentity;

        internal Service(AppwriteConfig config, IUserIdentity userIdentity)
	    {
            this.config = config;
            this.userIdentity = userIdentity;

            headers = new Dictionary<string, string>(new Dictionary<string, string>
                {
                    { "X-Appwrite-Project", config.AppwriteProjectID },
                    { "Content-Type", "application/json" }
                });
        }

        protected void ReadUserData()
        {
            userIdentity.GetUser(true);
        }

        protected string ToHttpQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder("?");

            bool first = true;

            foreach (string key in nvc.AllKeys)
            {
                foreach (string value in nvc.GetValues(key))
                {
                    if (!first)
                    {
                        sb.Append("&");
                    }

                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));

                    first = false;
                }
            }

            return sb.ToString();
        }
    }
}



