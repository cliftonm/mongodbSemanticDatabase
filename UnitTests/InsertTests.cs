using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class InsertTests
	{
		[TestMethod]
		public void InsertConcreteTypeTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetSimpleTestSchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void InsertDuplicateConcreteTypeTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetSimpleTestSchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> bson;
			
			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));

			Assert.IsTrue(bson[0].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(bson[1].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(bson[2].ToString().Contains("\"_ref\" : 1"));

			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson[0].ToString().Contains("\"_ref\" : 2"));
		}

		[TestMethod]
		public void InsertHierarchyTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetTestHierarchySchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> bson;
			
			bson = sd.GetAll("name");
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"name\" : \"Greece\""));

			bson = sd.GetAll("countryName");
			Assert.IsTrue(bson.Count == 3);

			bson = sd.GetAll("countryCode");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1"));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20"));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30"));
		}

		[TestMethod]
		public void InsertDuplicateHierarchyTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetTestHierarchySchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> bson;

			bson = sd.GetAll("name");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"name\" : \"United States\""));

			bson = sd.GetAll("countryName");
			Assert.IsTrue(bson.Count == 3);

			bson = sd.GetAll("countryCode");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1"));

			// Duplicate insert:
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			bson = sd.GetAll("name");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"_ref\" : 2"));
			Assert.IsTrue(bson[1].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(bson[2].ToString().Contains("\"_ref\" : 1"));

			bson = sd.GetAll("countryName");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"_ref\" : 2"));
			Assert.IsTrue(bson[1].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(bson[2].ToString().Contains("\"_ref\" : 1"));

			bson = sd.GetAll("countryCode");
			Assert.IsTrue(bson.Count == 3);
			Assert.IsTrue(bson[0].ToString().Contains("\"_ref\" : 2"));
			Assert.IsTrue(bson[1].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(bson[2].ToString().Contains("\"_ref\" : 1"));
		}
	}
}
