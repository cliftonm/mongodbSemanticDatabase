using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	public static class Helpers
	{
		public static SemanticDatabase CreateCleanDatabase()
		{
			SemanticDatabase sd = new SemanticDatabase();
			sd.DropDatabase("testdb");
			sd.Open("testdb");

			return sd;
		}

		public static Schema InstantiateSchema(string json)
		{
			Schema target = new Schema();
			JsonConvert.PopulateObject(json, target);

			return target;
		}
	}
}
