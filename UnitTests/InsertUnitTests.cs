using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

			List<string> json = sd.GetAll("countryCodeLookup");

			Assert.IsTrue(json[0].Contains("{ \"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(json[1].Contains("{ \"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].Contains("{ \"value\" : 30, \"name\" : \"Greece\""));
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

			List<string> json;
			
			json = sd.GetAll("countryCodeLookup");

			Assert.IsTrue(json[0].Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(json[1].Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].Contains("\"value\" : 30, \"name\" : \"Greece\""));

			Assert.IsTrue(json[0].Contains("\"_ref\" : 1 }"));
			Assert.IsTrue(json[1].Contains("\"_ref\" : 1 }"));
			Assert.IsTrue(json[2].Contains("\"_ref\" : 1 }"));

			sd.Insert(schema, JObject.Parse("{value: 1, name: 'United States'}"));
			json = sd.GetAll("countryCodeLookup");
			Assert.IsTrue(json[0].Contains("\"_ref\" : 2"));
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

			List<string> json;
			
			json = sd.GetAll("name");

			Assert.IsTrue(json[0].Contains("\"name\" : \"United States\""));
			Assert.IsTrue(json[1].Contains("\"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].Contains("\"name\" : \"Greece\""));

			json = sd.GetAll("countryName");
			Assert.IsTrue(json.Count==3);

			json = sd.GetAll("countryCode");
			Assert.IsTrue(json[0].Contains("\"value\" : 1"));
			Assert.IsTrue(json[1].Contains("\"value\" : 20"));
			Assert.IsTrue(json[2].Contains("\"value\" : 30"));
		}
	}
}
