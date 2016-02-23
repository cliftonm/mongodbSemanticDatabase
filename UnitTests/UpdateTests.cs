using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	/// <summary>
	/// Update tests are the most complicated--
	/// We can update a collection record if the ref count == 1,
	/// otherwise, we need to decouple the super-type FK and insert (or reference an existing) sub-type,
	/// and the previous sub-type reference count must be decremented.  Fun stuff.
	/// </summary>
	[TestClass]
	public class UpdateTests
	{
		[TestMethod]
		public void UpdateConcreteTypeTest()
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

			sd.Update(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"), BsonDocument.Parse("{value: 1, name: 'United States of America'}"));

			bson = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States of America\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		/// <summary>
		/// Test changing a concrete value at the bottom of the hierarcy.
		/// </summary>
		[TestMethod]
		public void UpdateBottomHierarchySingleReferenceTest()
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

			// This tests updating the bottom of the hierarchy, and since there are no other references, we can update the only instance.
			sd.Update(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"), BsonDocument.Parse("{value: 1, name: 'United States of America'}"));
			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States of America\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		/// <summary>
		/// Test changing the country code (the value field), which is at the top of the hierarchy.
		/// </summary>
		[TestMethod]
		public void UpdateTopHierarchySingleReferenceTest()
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

			// This tests updating the top of the hierachy, changing the country code from 1 to 3.
			sd.Update(schema, BsonDocument.Parse("{value: 1}"), BsonDocument.Parse("{value: 3}"));
			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 3);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 3, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		/// <summary>
		/// Test changing the country name when there are two references to the same country.
		/// </summary>
		[TestMethod]
		public void UpdateBottomHierarchyMultipleReferenceTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.GetTestHierarchySchema();

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, BsonDocument.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 30, name: 'Greece'}"));
			sd.Insert(schema, BsonDocument.Parse("{value: 40, name: 'United States'}"));		// The country name is in error.

			List<BsonDocument> bson;

			bson = sd.Query(schema);
			Assert.IsTrue(sd.GetAll("name").Count == 3);		// "United States" has a ref count of 2
			Assert.IsTrue(bson.Count == 4);

			// Fix the country name:
			sd.Update(schema, BsonDocument.Parse("{value: 40, name: 'United States'}"), BsonDocument.Parse("{value: 40, name: 'Romania'}"));
			bson = sd.Query(schema);
			Assert.IsTrue(sd.GetAll("name").Count == 4);		// Now we should have four unique country names
			Assert.IsTrue(bson.Count == 4);

			Assert.IsTrue(bson[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(bson[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
			Assert.IsTrue(bson[3].ToString().Contains("\"value\" : 40, \"name\" : \"Romania\""));
		}

		/// <summary>
		/// Test updating a person name, where first name and last name share the same subtype "name".
		/// </summary>
		[TestMethod]
		public void UpdateWithCommonSubTypeTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreatePersonSchema();
			BsonDocument originalData = BsonDocument.Parse("{firstName: '1', lastName: '2'}");
			string recordId = sd.Insert(schema, originalData);
			BsonDocument newData = BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}");
			sd.Update(schema, originalData, newData, recordId);
			
			List<BsonDocument> bson;
			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 1);
			Assert.IsTrue(bson[0].ToString().Contains("\"firstName\" : \"Marc\", \"lastName\" : \"Clifton\""));
		}

		/// <summary>
		/// Test updating a person name, where first name and last name share the same subtype "name" and the ref count on one of the name records is 2
		/// </summary>
		[TestMethod]
		public void UpdateWithCommonSubTypeMultipleReferenceTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.CreatePersonSchema();
			BsonDocument originalData1 = BsonDocument.Parse("{firstName: '1', lastName: '2'}");
			BsonDocument originalData2 = BsonDocument.Parse("{firstName: '2', lastName: '3'}");
			string recordId1 = sd.Insert(schema, originalData1);
			string recordId2 = sd.Insert(schema, originalData2);	// "2" is ref'd twice.
			
			BsonDocument newData1 = BsonDocument.Parse("{firstName: 'Marc', lastName: 'Clifton'}");
			sd.Update(schema, originalData1, newData1, recordId1);
			BsonDocument newData2 = BsonDocument.Parse("{firstName: 'Ian', lastName: 'Clifton'}");
			sd.Update(schema, originalData2, newData2, recordId2);
			List<BsonDocument> bson;
			bson = sd.Query(schema);
			Assert.IsTrue(bson.Count == 2);
			Assert.IsTrue(bson[0].ToString().Contains("\"firstName\" : \"Marc\", \"lastName\" : \"Clifton\""));
			Assert.IsTrue(bson[1].ToString().Contains("\"firstName\" : \"Ian\", \"lastName\" : \"Clifton\""));

			bson = sd.GetAll("name");
			Assert.IsTrue(bson.Count == 3);				// should only consist of "Ian", "Marc", and "Clifton".

			bson = sd.GetAll("lastName");
			Assert.IsTrue(bson.Count == 1);				// Only one "Clifton"
		}
	}
}
