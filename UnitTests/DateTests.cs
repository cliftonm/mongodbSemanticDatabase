using System;
using System.Collections.Generic;
using System.Linq;
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

		[TestMethod]
		public void MonthNameTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema monthLookupSchema = Helpers.CreateMonthNameLookupSchema();
			InstantiateMonthLookup(sd, monthLookupSchema);
			List<BsonDocument> docs = sd.QueryServerSide(monthLookupSchema);
			Assert.IsTrue(docs.Count == 12);
		}

		[TestMethod]
		public void DiscoverAssociationsTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema monthLookupSchema = Helpers.CreateMonthNameLookupSchema();
			Schema dateSchema = Helpers.CreatePureDateSchema();
			Schema personSchema = Helpers.CreatePersonSchema();
			List<CommonType> commonTypes = sd.DiscoverAssociations(new Schema[] { monthLookupSchema, dateSchema, personSchema });

			foreach(CommonType ct in commonTypes)
			{
				System.Diagnostics.Debug.WriteLine(String.Join(" <- ", ct.Schema1.GetTypeChain().Select(t=>t.Name)));
				System.Diagnostics.Debug.WriteLine(String.Join(" <- ", ct.Schema2.GetTypeChain().Select(t => t.Name)));
			}

			InstantiateMonthLookup(sd, monthLookupSchema);
			InstantiateDate(sd, dateSchema);
			InstantiatePerson(sd, personSchema);
		}

		protected void InstantiateMonthLookup(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{month: 1, monthName: 'January', monthAbbr: 'Jan'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 2, monthName: 'February', monthAbbr: 'Feb'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 3, monthName: 'March', monthAbbr: 'Mar'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 4, monthName: 'April', monthAbbr: 'Apr'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 5, monthName: 'May', monthAbbr: 'May'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 6, monthName: 'June', monthAbbr: 'Jun'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 7, monthName: 'July', monthAbbr: 'Jul'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 8, monthName: 'August', monthAbbr: 'Aug'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 9, monthName: 'September', monthAbbr: 'Sep'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 10, monthName: 'October', monthAbbr: 'Oct'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 11, monthName: 'November', monthAbbr: 'Nov'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 12, monthName: 'December', monthAbbr: 'Dec'}"));
		}

		protected void InstantiateDate(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{month: 8, day: 19, year: 1962}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 4, day: 1, year: 2016}"));
		}

		protected void InstantiatePerson(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			sd.Insert(schema, BsonDocument.Parse("{firstName: 'April', lastName: 'Jones'}"));
		}
	}
}
