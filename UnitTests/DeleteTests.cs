using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class DeleteTests
	{
		[TestMethod]
		public void DeleteSingleInstanceTest()
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

			sd.Delete(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));

			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 2);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void DeleteMultipleReferenceTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetSimpleTestSchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			// second reference:
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));

			List<BsonDocument> bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 3);

			// First delete:
			sd.Delete(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));

			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));

			// Second delete:
			sd.Delete(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));

			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 2);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void DeleteHierarchyTest()
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

			sd.Delete(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));

			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 2);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void DeleteMultipleReferenceHierarchyTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetTestHierarchySchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));

			// Insert a record that re-uses the country name.
			sd.Insert(schema, BsonDocument.Parse("{value: 2, name: 'United States'}"));

			List<BsonDocument> bson;

			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 4);

			// Delete just the re-use high-level type.
			sd.Delete(schema, BsonDocument.Parse("{value: 2, name: 'United States'}"));

			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		// TODO: We should not be able to delete a sub-type if it is referenced by
		// a super-type.  We need a master schema to know whether a sub-type has a super-type somewhere,
		// or we need to ask the DB for fields of the form "[subtype]Id", which would indicate that the
		// subtype is an FK in a supertype.
	}
}
