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

			List<string> associations = new List<string>();

			foreach(CommonType ct in commonTypes)
			{
				associations.Add(String.Join(" <- ", ct.Schema1.GetTypeChain().Select(t=>t.Name)));
				associations.Add(String.Join(" <- ", ct.Schema2.GetTypeChain().Select(t => t.Name)));
			}

			Assert.IsTrue(associations[0] == "month <- monthLookup");
			Assert.IsTrue(associations[1] == "month <- date");
			Assert.IsTrue(associations[2] == "name <- monthName <- monthLookup");
			Assert.IsTrue(associations[3] == "name <- firstName <- personName <- person");
			Assert.IsTrue(associations[4] == "name <- monthName <- monthLookup");
			Assert.IsTrue(associations[5] == "name <- lastName <- personName <- person");
			Assert.IsTrue(associations[6] == "name <- monthAbbr <- monthLookup");
			Assert.IsTrue(associations[7] == "name <- firstName <- personName <- person");
			Assert.IsTrue(associations[8] == "name <- monthAbbr <- monthLookup");
			Assert.IsTrue(associations[9] == "name <- lastName <- personName <- person");

			InstantiateMonthLookup(sd, monthLookupSchema);
			InstantiateTestDateRecords(sd, dateSchema);
			InstantiateTestPersonRecords(sd, personSchema);

			// A manual query to validate that the intrinsic association returns what we expect: The name of a person whose first name is the name of a date's month.

			List<BsonDocument> data = sd.GetCollection("firstName").Aggregate().Lookup("name", "nameId", "_id", "fname").Unwind("fname")
				.Lookup("monthName", "fname._id", "nameId", "monthName").Unwind("monthName")
				.Lookup("monthLookup", "monthName._id", "monthNameId", "monthLookup").Unwind("monthLookup")
				.Lookup("month", "monthLookup.monthId", "_id", "month").Unwind("month")
				.Lookup("date", "month._id", "monthId", "date").Unwind("date")
				.Lookup("day", "date.dayId", "_id", "day").Unwind("day")
				.Lookup("year", "date.yearId", "_id", "year").Unwind("year")
				.Project(Builders<BsonDocument>.Projection.Include("fname.name").Include("month.value").Include("day.value").Include("year.value").Exclude("_id")).ToList();

			Assert.IsTrue(data.Count == 1);
			Assert.IsTrue(data[0].ToString() == "{ \"fname\" : { \"name\" : \"April\" }, \"month\" : { \"value\" : 4 }, \"day\" : { \"value\" : 1 }, \"year\" : { \"value\" : 2016 } }");
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

		protected void InstantiateTestDateRecords(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{month: 8, day: 19, year: 1962}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 4, day: 1, year: 2016}"));
		}

		protected void InstantiateTestPersonRecords(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}"));
			sd.Insert(schema, BsonDocument.Parse("{firstName: 'April', lastName: 'Jones'}"));
		}
	}
}
