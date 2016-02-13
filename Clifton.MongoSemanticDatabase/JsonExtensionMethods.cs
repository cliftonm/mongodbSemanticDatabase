using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Clifton.MongoSemanticDatabase
{
	public static class JsonExtensionMethods
	{
		/// <summary>
		/// Returns true if the JObject, serialized as an unformatted string, contains the json string.
		/// </summary>
		public static bool Contains(this JObject jobj, string json)
		{
			return jobj.ToString(Formatting.None).Contains(json);
		}

		/// <summary>
		/// Returns the JObject serialized as an unformatted string (no whitespace, to CRLF's)
		/// </summary>
		public static string AsUnformattedString(this JObject jobj)
		{
			return jobj.ToString(Formatting.None);
		}
	}
}
