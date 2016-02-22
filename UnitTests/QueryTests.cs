using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class QueryTests
	{
		[TestMethod]
		public void ConcreteQueryTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetSimpleTestSchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			List<BsonDocument> records = sd.Query(schema);

			Assert.IsTrue(records.Count == 1);
			Assert.IsTrue(records[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
		}

		[TestMethod]
		public void HierarchicalQueryTest()
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

			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));

			bson = sd.QueryServerSide(schema);
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void SameSubtypeQueryTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreatePersonSchema();
			sd.InstantiateSchema(schema);
			sd.Insert(schema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			List<BsonDocument> records = sd.Query(schema);

			Assert.IsTrue(records.Count == 1);
			Assert.IsTrue(records[0].ToString().Contains("\"firstName\" : \"Marc\", \"lastName\" : \"Clifton\""));

			// Tests that name, which is referenced by firstname and lastname, generates the correct server-side query.
			records = sd.QueryServerSide(schema);
			Assert.IsTrue(records.Count == 1);
			Assert.IsTrue(records[0].ToString().Contains("\"firstName\" : \"Marc\", \"lastName\" : \"Clifton\""));
		}
	}
}
