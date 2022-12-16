using System;
using System.Collections.Generic;
using System.IO;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Utils;
using UnityEngine;

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

            // Fetches user info written to disk.
            user = FileUtilities.Read<User>(UserPath, config);
        }
    }
}



