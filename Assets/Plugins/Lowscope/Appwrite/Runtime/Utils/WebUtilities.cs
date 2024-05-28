using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Lowscope.AppwritePlugin.Utils
{
	public static class WebUtilities
	{
		public static bool IsEmailValid(string email)
		{
			Regex regex =
				new Regex(
					@"^[-!#$%&'*+\/0-9=?A-Z^_a-z`{|}~](\.?[-!#$%&'*+\/0-9=?A-Z^_a-z`{|}~])*@[a-zA-Z0-9](-*\.?[a-zA-Z0-9])*\.[a-zA-Z](-?[a-zA-Z0-9])+$");
			return regex.Match(email).Success;
		}

		public static string ToQueryString(NameValueCollection nvc)
		{
			IEnumerable<string> segments = from key in nvc.AllKeys
										from value in nvc.GetValues(key)
										select string.Format("{0}={1}", 
										WebUtility.UrlEncode(key),
										WebUtility.UrlEncode(value));
			return "?" + string.Join("&", segments);
		}
	}
}