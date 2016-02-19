using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;
using MongoDB.Driver;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class ExplicitAssociationTests
	{
		[TestMethod]
		public void PersonDateAssociationTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema dateSchema = Helpers.CreatePureDateSchema();
			Schema personSchema = Helpers.CreatePersonSchema();
			Schema personDateSchema = sd.Associate(personSchema, dateSchema);

			string personId = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			string dateId = sd.Insert(dateSchema, BsonDocument.Parse("{month: 8, day: 19, year: 1962}"));
			
			BsonDocument doc = new BsonDocument("personId", new ObjectId(personId));
			doc.Add("dateId", new ObjectId(dateId));
			doc.Add("forwardAssociationName", "birthdate");
			doc.Add("reverseAssociationName", "birthdate");
			sd.Insert(personDateSchema, doc);

			List<BsonDocument> docs = sd.Query(personDateSchema);
			Assert.IsTrue(docs.Count == 1);
		}

		[TestMethod]
		public void PersonPersonAssociationTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema personSchema = Helpers.CreatePersonSchema();
			Schema personPersonSchema = sd.Associate(personSchema, personSchema);

			string personId1 = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			string personId2 = sd.Insert(personSchema, BsonDocument.Parse("{firstName: 'Elisabeth', lastName: 'Clifton'}"));

			BsonDocument doc = new BsonDocument("person1Id", new ObjectId(personId1));
			doc.Add("person2Id", new ObjectId(personId2));
			doc.Add("forwardAssociationName", "son");			// Marc is the son of Elisabeth
			doc.Add("reverseAssociationName", "mother");		// Elisabeth is the mother of Marc
			sd.Insert(personPersonSchema, doc);

			List<BsonDocument> docs = sd.Query(personPersonSchema);
			Assert.IsTrue(docs.Count == 1);
		}
	}
}
