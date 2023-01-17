using System;
using System.IO;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Utils;
using UnityEngine;

namespace Lowscope.AppwritePlugin.Identity
{
	public class EncryptedFileUserIdentity: IUserIdentity
	{
        protected string UserPath => Path.Combine(Application.persistentDataPath, "user.json");

        private AppwriteConfig config { get; set; }

        private User user;

        public EncryptedFileUserIdentity(AppwriteConfig config)
		{
            this.config = config;
		}

        public void ClearUserIdentity()
        {
            user = null;
            if (File.Exists(UserPath))
                File.Delete(UserPath);
        }

        public void StoreUserIdentity(User user)
        {
            FileUtilities.Write(user, UserPath, config);
            this.user = user;
        }

        public User GetUser(bool readFromPersistentStorage = false)
        {
            if (readFromPersistentStorage)
            {
                user = FileUtilities.Read<User>(UserPath, config);
            }
            return user;
        }
    }
}

