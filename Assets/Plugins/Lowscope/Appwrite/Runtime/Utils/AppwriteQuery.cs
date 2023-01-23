using System;

namespace Lowscope.AppwritePlugin.Utils
{
	public class AppwriteQuery
	{

		public static string Equal(string attribute, string[] value)
		{
			return AddQuery(attribute, "equal", value);
		}

		public static string Equal(string attribute, string value)
		{
			return Equal(attribute, new string[] { value });
		}

		public static string NotEqual(string attribute, string[] value)
		{
			return AddQuery(attribute, "notEqual", value);
		}

		public static string NotEqual(string attribute, string value)
		{
			return NotEqual(attribute, new string[] { value });
		}

		public static string LessThan(string attribute, string[] value)
		{
			return AddQuery(attribute, "lessThan", value);
		}

        public static string LessThan(string attribute, string value)
        {
            return LessThan(attribute, new string[] { value });
        }

		public static string LessThanEqual(string attribute, string[] value)
		{
			return AddQuery(attribute, "lessThanEqual", value);
		}

		public static string GreaterThan(string attribute, string[] value)
		{
			return AddQuery(attribute, "greaterThan", value);
		}

		public static string GreaterThanEqual(string attribute, string[] value)
		{
			return AddQuery(attribute, "greaterThanEqual", value);
		}

		public static string Search(string attribute, string[] value)
		{
			return AddQuery(attribute, "search", value);
		}

		public static string CursorAfter(string documentId)
		{
			return "cursorAfter(\"" + documentId + "\")";
		}

        public static string CursorBefore(string documentId)
        {
            return "cursorBefore(\"" + documentId + "\")";
        }

		public static string OrderAsc(string attribute)
		{
			return "orderAsc(\n" + attribute + "\")";
		}

        public static string OrderDesc(string attribute)
        {
            return "orderDesc(\n" + attribute + "\")";
        }

        public static string Limit(int limit)
        {
            return "limit(\n" + limit + "\")";
        }

		public static string Offset(int offset)
		{
			return "offset(\n" + offset + "\")";
        }

        private static string AddQuery(string attribute, string method, string[] values)
		{
			return method + "(\"" + attribute + "\",[\"" + string.Join("\",\"", values) + "\"])";
		}
	}
}

