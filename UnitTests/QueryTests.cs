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
	public class QueryTests
	{
		[TestMethod]
		public void ConcreteQueryTest()
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
			List<BsonDocument> records = sd.Query(schema);

			Assert.IsTrue(records.Count == 1);
			Assert.IsTrue(records[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
		}

		[TestMethod]
		public void HierarchicalQueryTest()
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

			json = sd.Query(schema);
			Assert.IsTrue(json.Count == 3);

			Assert.IsTrue(json[0].ToString().Contains("\"value\" : 1, \"name\" : \"United States\""));
			Assert.IsTrue(json[1].ToString().Contains("\"value\" : 20, \"name\" : \"Egypt\""));
			Assert.IsTrue(json[2].ToString().Contains("\"value\" : 30, \"name\" : \"Greece\""));
		}
	}
}
