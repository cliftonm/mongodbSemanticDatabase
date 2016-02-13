using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class DateTests
	{
		[TestMethod]
		public void CreateDateSchemaTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreateDateSchema();
			sd.InstantiateSchema(schema);
			List<string> collections = sd.GetCollections();
			Assert.IsTrue(collections.Count == 6);
			Assert.IsTrue(collections.Contains("date"));
			Assert.IsTrue(collections.Contains("month"));
			Assert.IsTrue(collections.Contains("day"));
			Assert.IsTrue(collections.Contains("year"));
			Assert.IsTrue(collections.Contains("name"));
			Assert.IsTrue(collections.Contains("monthName"));
		}

		[TestMethod]
		public void InsertOnlyMonthTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreateDateSchema();
			sd.InstantiateSchema(schema);
			List<string> collections = sd.GetCollections();
			Assert.IsTrue(collections.Count == 6);

			sd.Insert(schema, BsonDocument.Parse("{month: 8, monthName: 'August'}"));

			List<BsonDocument> docs = sd.Query(schema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString()=="{ \"month\" : 8, \"monthName\" : \"August\" }");

			docs = sd.QueryServerSide(schema);
			Assert.IsTrue(docs.Count == 0, "Partial semantic instance expected to fail when the $unwind aggregator encounters an empty array.");
		}

		[TestMethod]
		public void InsertFullDateTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreateDateSchema();
			sd.InstantiateSchema(schema);
			List<string> collections = sd.GetCollections();
			Assert.IsTrue(collections.Count == 6);

			sd.Insert(schema, BsonDocument.Parse("{month: 8, monthName: 'August', day: 19, year: 1962}"));

			List<BsonDocument> docs = sd.Query(schema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"month\" : 8, \"monthName\" : \"August\", \"day\" : 19, \"year\" : 1962 }");

			docs = sd.QueryServerSide(schema);
			Assert.IsTrue(docs.Count == 1);
			Assert.IsTrue(docs[0].ToString() == "{ \"month\" : 8, \"monthName\" : \"August\", \"day\" : 19, \"year\" : 1962 }");
		}
	}
}
