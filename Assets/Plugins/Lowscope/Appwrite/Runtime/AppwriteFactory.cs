using System;
using Lowscope.AppwritePlugin.Accounts;
using Lowscope.AppwritePlugin.Identity;

namespace Lowscope.AppwritePlugin
{
    public class AppwriteFactory
    {
        private static AppwriteFactory _instance;

        public static AppwriteFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppwriteFactory();
                }
                return _instance;
            }
        }

        private AppwriteFactory()
        {
        }

        public IUserIdentity UserIdentity {get; set;}

        public Storage.Storage GetStorage(AppwriteConfig config)
        {
            return new Storage.Storage(config, UserIdentity);
        }

        public Account GetAccount(AppwriteConfig config)
        {
            return new Account(config, UserIdentity);
        }

        public Database.Database GetDatabase(AppwriteConfig config)
        {
            return new Database.Database(config, UserIdentity);
        }
    }
}

