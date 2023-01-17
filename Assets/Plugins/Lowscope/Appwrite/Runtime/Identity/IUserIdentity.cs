using System;
using Lowscope.AppwritePlugin.Accounts.Model;

namespace Lowscope.AppwritePlugin.Identity
{

    public interface IUserIdentity
    {
        public void StoreUserIdentity(User user);
        public void ClearUserIdentity();

        public User GetUser(bool readFromPresistentStorage = false);
    }
}

