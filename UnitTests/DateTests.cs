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

			sd.Insert(schema, BsonDocument.Parse("{day: 8, month: 'August'}"));
		}
	}
}
