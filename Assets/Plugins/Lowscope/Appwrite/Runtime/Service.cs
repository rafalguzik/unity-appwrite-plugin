using System;
using System.Collections.Generic;
using System.IO;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Utils;
using UnityEngine;
using System.Net;

namespace Lowscope.AppwritePlugin
{
    public abstract class Service
    {
        protected AppwriteConfig config;
        protected readonly Dictionary<string, string> headers;

        protected User user;

        protected string UserPath => Path.Combine(Application.persistentDataPath, "user.json");

        internal Service(AppwriteConfig config)
	    {
            this.config = config;

            headers = new Dictionary<string, string>(new Dictionary<string, string>
                {
                    { "X-Appwrite-Project", config.AppwriteProjectID },
                    { "Content-Type", "application/json" }
                });

            //ReadUserData();
        }

        protected void ReadUserData()
        {
            // Fetches user info written to disk.
            user = FileUtilities.Read<User>(UserPath, config);
        }

        public void SetUser(User user)
        {
            this.user = user;
        }

        protected void ProcessHttpStatusCode(HttpStatusCode code, string message = "")
        {
            switch (code)
            {
                case 0:
                    throw new AppwriteException(AppwriteException.Error.UnknownError, message);
                case HttpStatusCode.Unauthorized:
                    throw new AppwriteException(AppwriteException.Error.NotAuthorized, message);
                case HttpStatusCode.NotFound:
                    throw new AppwriteException(AppwriteException.Error.NotFound, message);

            }
        } 
    }
}



