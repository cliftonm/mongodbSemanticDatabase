using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class SemanticDatabaseUnitTests
	{
		[TestMethod]
		public void CreateConcreteCollectionTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema("{name: 'countryCode'}");
			sd.InstantiateSchema(schema);
			List<string> collections = sd.GetCollections();
			Assert.IsTrue(collections.Count == 1, "Expected 1 collection.");
			Assert.IsTrue(collections[0] == "countryCode", "Collection does not match expected name");
		}

		[TestMethod]
		public void CreateSpecializedCollectionTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema(@"
			{
				name: 'phoneNumber', 
				subtypes:
				[
					{name: 'countryCode'},
					{name: 'areaCode'},
					{name: 'exchange'},
					{name: 'subscriberId'},
				]
			}");
			sd.InstantiateSchema(schema);
			List<string> collections = sd.GetCollections();
			Assert.IsTrue(collections.Count == 5, "Expected 5 collections.");
			Assert.IsTrue(collections.Contains("phoneNumber"));
			Assert.IsTrue(collections.Contains("countryCode"));
			Assert.IsTrue(collections.Contains("areaCode"));
			Assert.IsTrue(collections.Contains("exchange"));
			Assert.IsTrue(collections.Contains("subscriberId"));
		}
	}
}
