using System;
using Lowscope.AppwritePlugin.Accounts;

namespace Lowscope.AppwritePlugin
{
    public class AppwriteFactory
    {
        public static Storage.Storage GetStorage(AppwriteConfig config)
        {
            return new Storage.Storage(config);
        }

        public static Account GetAccount(AppwriteConfig config)
        {
            return new Account(config);
        }

        public static Database.Database GetDatabase(AppwriteConfig config)
        {
            return new Database.Database(config);
        }
    }
}

