using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Bson;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	[TestClass]
	public class InsertUnitTests
	{
		[TestMethod]
		public void InsertConcreteTypeTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema(@"
			{
				name: 'countryCodeLookup', 
				concreteTypes:
				{
					value: 'System.Int32',
					name: 'System.String'
				}
			}");

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, JObject.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, JObject.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> json = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(json.Count == 3);

			Assert.IsTrue(json[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(json[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}

		[TestMethod]
		public void InsertDuplicateConcreteTypeTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema(@"
			{
				name: 'countryCodeLookup', 
				concreteTypes:
				{
					value: 'System.Int32',
					name: 'System.String'
				}
			}");

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 1, "Collection should be length of 1.");

			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, JObject.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, JObject.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> json;
			
			json = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(json.Count == 3);

			Assert.IsTrue(json[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(json[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));

			Assert.IsTrue(json[0].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(json[1].ToString().Contains("\"_ref\" : 1"));
			Assert.IsTrue(json[2].ToString().Contains("\"_ref\" : 1"));

			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			json = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(json[0].ToString().Contains("\"_ref\" : 2"));
		}

		[TestMethod]
		public void InsertHierarchyTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema(@"
			{
				name: 'countryCode', 
				concreteTypes:
				{
					value: 'System.Int32',
				},
				subtypes: 
				[
					{
						name: 'countryName', 
						subtypes: 
						[
							{
								name: 'name', 
								concreteTypes:
								{
									value: 'System.String'
								}
							}
						]
					}
				]
			}");

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			sd.Insert(schema, JObject.Parse("{value: 20, name: 'Egypt'}"));
			sd.Insert(schema, JObject.Parse("{value: 30, name: 'Greece'}"));

			List<BsonDocument> json;
			
			json = sd.GetAll("name");
			Assert.IsTrue(json.Count == 3);

			Assert.IsTrue(json[0].ToString().Contains("\"name\" : \"United States\""));
			Assert.IsTrue(json[1].ToString().Contains("\"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].ToString().Contains("\"name\" : \"Greece\""));

			json = sd.GetAll("countryName");
			Assert.IsTrue(json.Count == 3);

			json = sd.GetAll("countryCode");
			Assert.IsTrue(json.Count == 3);
			Assert.IsTrue(json[0].ToString().Contains("\"value\" : 1"));
			Assert.IsTrue(json[1].ToString().Contains("\"value\" : 20"));
			Assert.IsTrue(json[2].ToString().Contains("\"value\" : 30"));
		}

		[TestMethod]
		public void InsertDuplicateHierarchyTest()
		{
			SemanticDatabase sd = Helpers.CreateCleanDatabase();
			Assert.IsTrue(sd.GetCollections().Count == 0, "Collection should be 0 length.");
			Schema schema = Helpers.InstantiateSchema(@"
			{
				name: 'countryCode', 
				concreteTypes:
				{
					value: 'System.Int32',
				},
				subtypes: 
				[
					{
						name: 'countryName', 
						subtypes: 
						[
							{
								name: 'name', 
								concreteTypes:
								{
									value: 'System.String'
								}
							}
						]
					}
				]
			}");

			sd.InstantiateSchema(schema);
			Assert.IsTrue(sd.GetCollections().Count == 3, "Collection should be length of 3.");
			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));

			List<BsonDocument> json;

			json = sd.GetAll("name");
			Assert.IsTrue(json.Count == 1);
			Assert.IsTrue(json[0].ToString().Contains("\"name\" : \"United States\""));

			json = sd.GetAll("countryName");
			Assert.IsTrue(json.Count == 1);

			json = sd.GetAll("countryCode");
			Assert.IsTrue(json.Count == 1);
			Assert.IsTrue(json[0].ToString().Contains("\"value\" : 1"));

			// Duplicate insert:
			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			json = sd.GetAll("name");
			Assert.IsTrue(json.Count == 1);
			Assert.IsTrue(json[0].ToString().Contains("\"_ref\" : 2"));

			json = sd.GetAll("countryName");
			Assert.IsTrue(json.Count == 1);
			Assert.IsTrue(json[0].ToString().Contains("\"_ref\" : 2"));

			json = sd.GetAll("countryCode");
			Assert.IsTrue(json.Count == 1);
			Assert.IsTrue(json[0].ToString().Contains("\"_ref\" : 2"));
		}
	}
}
