using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using Lowscope.AppwritePlugin.Accounts.Model;
using Lowscope.AppwritePlugin.Identity;
using Lowscope.AppwritePlugin.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.XR;
using WebRequest = Lowscope.AppwritePlugin.Utils.WebRequest;

namespace Lowscope.AppwritePlugin.Accounts
{
	public class Account: Service
	{
		public Action<User> OnLogin = delegate {  };
		public Action OnLogout = delegate {  };

		private DateTime lastRegisterRequestDate;

		private bool validatedSession = false;


		internal Account(AppwriteConfig config, IUserIdentity userIdentity): base(config, userIdentity)
		{
			
		}

		private void StoreUserToDisk(User user)
		{
			userIdentity.StoreUserIdentity(user);

			if (validatedSession) 
				return;
			
			validatedSession = true;
			
			OnLogin(user);
		}

		private void ClearUserDataFromDisk()
		{
			userIdentity.ClearUserIdentity();
            validatedSession = false;
            OnLogout();
		}

		private async UniTask<bool> RequestUserInfo()
		{
			if (userIdentity.GetUser() == null)
				return false;

			string url = $"{config.AppwriteURL}/account";
			using var request = new WebRequest(EWebRequestType.GET, url, headers, userIdentity.GetUser().Cookie);
			var json = await request.Send();

			JObject parsedData = JObject.Parse(json);

			User user = userIdentity.GetUser();
			user.Name = (string)parsedData.GetValue("name");
			user.EmailVerified = (bool)parsedData.GetValue("emailVerification");
			StoreUserToDisk(user);

			return true;
		}

		/// <summary>
		/// Creates a session. Cookie is stored on disk and other requests will use the current session.
		/// </summary>
		public async UniTask<User> Login(string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
				//return (null, ELoginResponse.MissingCredentials);
				throw new AppwriteException("Missing Credentials", ErrorType.MissingCredentials);

			if (userIdentity.GetUser() != null)
			{
				if (userIdentity.GetUser().Email == email)
					return userIdentity.GetUser();

				await Logout();
			}

			JObject obj = new JObject(
				new JProperty("email", email),
				new JProperty("password", password));

			byte[] bytes = Encoding.UTF8.GetBytes(obj.ToString());

			string url = $"{config.AppwriteURL}/account/sessions";

			using var request = new WebRequest(EWebRequestType.POST, url, headers, userIdentity.GetUser()?.Cookie, bytes);

			var body = await request.Send();

			JObject parsedData = JObject.Parse(body);

			User user = new User
			{
				Id = (string)parsedData.GetValue("userId"),
				Email = (string)parsedData.GetValue("providerUid"),
				Cookie = request.ExtractCookie()
			};

			// Attempts to get account info to fill in additional user data such as 
			// If email is verified and Name.
			if (!await RequestUserInfo())
                throw new AppwriteException("Login Failed", ErrorType.Failed);

            StoreUserToDisk(user);
			return user;
		}

		/// <summary>
		/// Send a register request. Will automatically login afterwards. Do note that a validation email is not
		/// send automatically. You have to call the specific function for it after registering.
		/// </summary>
		public async UniTask<User> Register(string id, string name, string email, string password)
		{
			if (!WebUtilities.IsEmailValid(email))
				throw new AppwriteException("Invalid Email", ErrorType.InvalidEmail);
				
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
				throw new AppwriteException("Missing credentials", ErrorType.MissingCredentials);

			if (userIdentity.GetUser() != null)
				return userIdentity.GetUser();

            if ((DateTime.Now - lastRegisterRequestDate).Duration().TotalMinutes < config.RegisterTimeoutMinutes)
				throw new AppwriteException("Timeout", ErrorType.Timeout);

			JObject obj = new JObject(
				new JProperty("userId", id),
				new JProperty("email", email),
				new JProperty("password", password),
				new JProperty("name", name));

			byte[] bytes = Encoding.UTF8.GetBytes(obj.ToString());

			string url = $"{config.AppwriteURL}/account";

			using var request = new WebRequest(EWebRequestType.POST, url, headers, userIdentity.GetUser()?.Cookie, bytes);

			var body = await request.Send();

			var user = await Login(email, password);
			return user;
		}

		/// <summary>
		/// Clears current session and removes any stored data regarding the user.
		/// </summary>
		/// <returns></returns>
		public async UniTask<bool> Logout()
		{
			if (userIdentity.GetUser() == null)
				return false;

			// Remove current session
			string url = $"{config.AppwriteURL}/account/sessions/current";
			using var request = new WebRequest(EWebRequestType.DELETE, url, headers, userIdentity.GetUser().Cookie);
			await request.Send();

			ClearUserDataFromDisk();

			return true;
		}

		/// <summary>
		/// Obtain a JWT, that can be used to validate your session with external services.
		/// </summary>
		public async UniTask<string> ObtainJwt(bool fromCache = true)
		{
			if (userIdentity.GetUser() == null)
				return "";

			if (fromCache && !string.IsNullOrEmpty(userIdentity.GetUser().Jwt))
				if ((DateTime.Now - userIdentity.GetUser().JwtProvideDate).Duration().TotalMinutes < config.JwtExpireMinutes)
					return userIdentity.GetUser().Jwt;

			string url = $"{config.AppwriteURL}/account/jwt";
			using var request = new WebRequest(EWebRequestType.POST, url, headers, userIdentity.GetUser().Cookie);
			var json = await request.Send();

			JObject parsedData = JObject.Parse(json);

			string jwt = (string)parsedData.GetValue("jwt");
            userIdentity.GetUser().Jwt = jwt;

            // Remove minute to account for latency.
            userIdentity.GetUser().JwtProvideDate = DateTime.Now - TimeSpan.FromMinutes(1);

			return jwt;
		}

		/// <summary>
		/// Sends a verification mail to the user. Provided with the url that is set in the config file.
		/// Read more about verification emails in the Appwrite documentation.
		/// </summary>
		public async UniTask<bool> RequestVerificationMail()
		{
			if (userIdentity.GetUser() == null)
				throw new AppwriteException("Not logged in.", ErrorType.NotLoggedIn);

			if (userIdentity.GetUser().EmailVerified)
				throw new AppwriteException("Already Verified", ErrorType.AlreadyVerified);

			if (userIdentity.GetUser().LastEmailRequestDate != default &&
			    (DateTime.Now - userIdentity.GetUser().LastEmailRequestDate).Duration().TotalMinutes < config.VerifyEmailTimeoutMinutes)
				throw new AppwriteException("Timeout", ErrorType.Timeout);

			if (string.IsNullOrEmpty(config.VerifyEmailURL))
				throw new AppwriteException("No URL Specified", ErrorType.NoURLSpecified);

			JObject obj = new JObject(new JProperty("url", config.VerifyEmailURL));
			byte[] bytes = Encoding.UTF8.GetBytes(obj.ToString());

			string url = $"{config.AppwriteURL}/account/verification";
			using var request = new WebRequest(EWebRequestType.POST, url, headers, userIdentity.GetUser().Cookie, bytes);

			try
			{
				var json = await request.Send();
			} catch (AppwriteException e)
			{
				if (e.ResponseCode != 0)
				{
					ClearUserDataFromDisk();
					throw new AppwriteException("Not Logged it", ErrorType.NotLoggedIn);
				} else
				{
					throw e;
				}

                
            }

			// Session has become invalid, not able to utilize session anymore.
			//switch (httpStatusCode)
			//{
			//	case HttpStatusCode.Unauthorized:
			//	case HttpStatusCode.NotFound:
			//		ClearUserDataFromDisk();
			//		return EEmailVerifyResponse.NotLoggedIn;
			//}

			User user = userIdentity.GetUser();
            user.LastEmailRequestDate = DateTime.Now;
			StoreUserToDisk(user);

			return true;
		}

		/// <summary>
		/// Obtains user information
		/// </summary>
		/// <param name="fromServer">Do we want to get the user information from the server?
		/// Can be useful to verify if session is still valid.</param>
		/// <returns></returns>
		public async UniTask<User> GetUser(bool fromServer = false)
		{
			if (userIdentity.GetUser() == null)
				return null;

			if (!fromServer)
				return userIdentity.GetUser();

			if (await RequestUserInfo())
				return userIdentity.GetUser();

			return null;
		}

		public async UniTask<User> CreateAnonymousSession()
		{
			ReadUserData();

			if (userIdentity.GetUser() != null) return userIdentity.GetUser();


            string url = $"{config.AppwriteURL}/account/sessions/anonymous";

            using var request = new WebRequest(EWebRequestType.POST, url, headers, userIdentity.GetUser()?.Cookie);
			request.SetTimeout(30);

            var body = await request.Send();

            JObject parsedData = JObject.Parse(body);

            User user = new User
            {
                Id = (string)parsedData.GetValue("userId"),
                Email = (string)parsedData.GetValue("providerUid"),
                Cookie = request.ExtractCookie()
            };
			
			StoreUserToDisk(user);

			// Attempts to get account info to fill in additional user data such as 
			// If email is verified and Name.
			if (!await RequestUserInfo())
                throw new AppwriteException("Creating anonymous session failed", ErrorType.Failed);


            return user;
        }

		public async UniTask<User> ConvertAnonymousUser(string email, string password)
		{
			ReadUserData();

            if (!WebUtilities.IsEmailValid(email))
                throw new AppwriteException("Invalid Email", ErrorType.InvalidEmail);

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                throw new AppwriteException("Missing credentials", ErrorType.MissingCredentials);

			if (password.Length < 8)
			{
				throw new AppwriteException("Password too short", ErrorType.MissingCredentials);
			}


			string url = $"{config.AppwriteURL}/account/email";

			JObject obj = new JObject(
				new JProperty("email", email),
				new JProperty("password", password)
			);

            byte[] bytes = Encoding.UTF8.GetBytes(obj.ToString());


			using var request = new WebRequest(EWebRequestType.PATCH, url, headers, userIdentity.GetUser()?.Cookie, bytes);
			request.SetTimeout(30);

            var body = await request.Send();

            JObject parsedData = JObject.Parse(body);

			Debug.Log("{ConvertAnonymousUser} response: " + parsedData);

            var user = await Login(email, password);
			return user;

            //User user = new User
            //{
            //    Id = (string)parsedData.GetValue("userId"),
            //    Email = (string)parsedData.GetValue("providerUid"),
            //    Cookie = request.ExtractCookie()
            //};

            //// Attempts to get account info to fill in additional user data such as 
            //// If email is verified and Name.
            //if (!await RequestUserInfo())
            //    throw new AppwriteException("Login Failed", ErrorType.Failed);

            //StoreUserToDisk(user);
            //return user;
        }
    }
}