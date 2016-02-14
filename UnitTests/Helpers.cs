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
				[
					{name: 'value', type: 'System.Int32'}
				],
				subtypes: 
				[
					{
						name: 'countryName', 
						subtypes: 
						[
							{
								name: 'name', 
								concreteTypes:
								[
									{name: 'name', type: 'System.String'},
								]
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
				[
					{name: 'value', type: 'System.Int32'},
					{name: 'name', type: 'System.String'},
				]
			}");
		}

		public static Schema CreateDateSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'date', 
				subTypes:
				[
					{
						name: 'month',
						concreteTypes:
						[
							{name: 'value', alias: 'month', type: 'System.Int32'}
						],
						subtypes:
						[
							{
								name: 'monthName',
								subtypes:
								[
									{
										name: 'name',
										concreteTypes:
										[
											{name: 'name', alias: 'monthName', type: 'System.String'}
										]
									}
								]
							}
						]
					},
					{
						name: 'day',
						concreteTypes:
						[
							{name: 'value', alias: 'day', type: 'System.Int32'}
						]
					},
					{
						name: 'year',
						concreteTypes:
						[
							{name: 'value', alias: 'year', type: 'System.Int32'}
						]
					}
				]
			}");
		}

		public static Schema CreatePureDateSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'date', 
				subTypes:
				[
					{
						name: 'month',
						concreteTypes:
						[
							{name: 'value', alias: 'month', type: 'System.Int32'}
						],
					},
					{
						name: 'day',
						concreteTypes:
						[
							{name: 'value', alias: 'day', type: 'System.Int32'}
						]
					},
					{
						name: 'year',
						concreteTypes:
						[
							{name: 'value', alias: 'year', type: 'System.Int32'}
						]
					}
				]
			}");
		}

		public static Schema CreateMonthNameLookupSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'monthLookup',
				subtypes:
				[
					{
						name: 'month',
						concreteTypes:
						[
							{name: 'value', alias: 'month', type: 'System.Int32'}
						],
					},
					{
						name: 'monthName',
						subtypes:
						[
							{
								name: 'name',
								concreteTypes:
								[
									{name: 'name', alias: 'monthName', type: 'System.String'}
								]
							}
						]
					},
					{
						name: 'monthAbbr',
						subtypes:
						[
							{
								name: 'name',
								concreteTypes:
								[
									{name: 'name', alias: 'monthAbbr', type: 'System.String'}
								]
							}
						]
					}
				]
			}");
		}

		public static Schema CreatePersonSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'person', 
				subtypes:
				[
					{
						name: 'personName',
						subtypes:
						[
							{
								name: 'firstName',
								subtypes:
								[
									{
									name: 'name',
									concreteTypes:
									{
										name: 'System.String'
									}
								]
							},
							{
								name: 'lastName',
								subtypes:
								[
									{
									name: 'name',
									concreteTypes:
									{
										name: 'System.String'
									}
								]
							},
						]
					}
				]
			}");
		}

		public static Schema CreatePhoneNumberSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'phoneNumber', 
				subtypes:
				[
					{
						name: 'areaCode',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'exchange',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'subscriberId',
						concreteTypes:
						{
							value: 'System.String'
						}
					}
				]
			}");
		}

		public static Schema CreateAddressSchema()
		{
			return Helpers.InstantiateSchema(@"
			{
				name: 'address', 
				subtypes:
				[
					{
						name: 'streetPrefix',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'streetName',
						subtypes:
						[
							{
							name: 'name',
							concreteTypes:
							{
								name: 'System.String'
							}
						]
					},
					{
						name: 'streetPostfix',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'boxNumber',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'city',
						concreteTypes:
						{
							value: 'System.String'
						}
					},
					{
						name: 'state',
						concreteTypes:
						{
							abbr: 'System.String'
						},
						subtypes:
						[
							{
							name: 'name',
							concreteTypes:
							{
								name: 'System.String'
							}
						]
					},
					{
						name: 'zipCode',
						concreteTypes:
						{
							zip: 'System.String'
							zip4: 'System.String'
						}
					},
				]
			}");
		}

	}
}
