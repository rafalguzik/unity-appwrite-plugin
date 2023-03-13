using System;
using System.Collections.Generic;
using System.IO;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Utils;
using UnityEngine;
using System.Net;
using Lowscope.AppwritePlugin.Identity;

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
    }
}



