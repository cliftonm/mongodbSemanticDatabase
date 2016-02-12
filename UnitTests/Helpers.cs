using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.MongoSemanticDatabase;

namespace UnitTests
{
	public static class Helpers
	{
		public static SemanticDatabase CreateCleanDatabase()
		{
			SemanticDatabase sd = new SemanticDatabase();
			sd.DropDatabase("testdb");
			sd.Open("testdb");

			return sd;
		}

		public static Schema InstantiateSchema(string json)
		{
			Schema target = new Schema();
			JsonConvert.PopulateObject(json, target);

			return target;
		}

		public static Schema GetTestHierarchySchema()
		{
			return Helpers.InstantiateSchema(@"
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
									name: 'System.String'
								}
							}
						]
					}
				]
			}");
		}

		public static Schema GetSimpleTestSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'countryCodeLookup', 
				concreteTypes:
				{
					value: 'System.Int32',
					name: 'System.String'
				}
			}");
		}
	}
}
