using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class SchemaTests
	{
		[TestMethod]
		public void GetSchemaTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetTestHierarchySchema();
			sd.InstantiateSchema(schema);

			Schema fromDbSchema = sd.DiscoverSchema("countryCode");
		}
	}
}
