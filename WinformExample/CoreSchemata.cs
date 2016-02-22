using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public static class CoreSchemata
	{
		public static Schema InstantiateSchema(string json)
		{
			Schema target = new Schema();
			JsonConvert.PopulateObject(json, target);

			return target;
		}

		public static Schema CreateDateSchema()
		{
			return InstantiateSchema(@"
			{
				name: 'date', 
				subTypes:
				[
					{
						name: 'month',
						concreteTypes:
						[
							{name: 'value', alias: 'month', type: 'System.Int32'}
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

		public static Schema CreateMonthNameLookupSchema()
		{
			return InstantiateSchema(@"
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
			return InstantiateSchema(@"
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
										alias: 'fname',
										concreteTypes:
										[
											{name: 'name', alias: 'firstName', type: 'System.String'}
										]
									}
								]
							},
							{
								name: 'lastName',
								subtypes:
								[
									{
										name: 'name',
										alias: 'lname',
										concreteTypes:
										[
											{name: 'name', alias: 'lastName', type: 'System.String'}
										]
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
			return InstantiateSchema(@"
			{
				name: 'phoneNumber', 
				subtypes:
				[
					{
						name: 'areaCode',
						concreteTypes:
						[
							{name: 'value', alias: 'areaCode', type: 'System.String'}
						]
					},
					{
						name: 'exchange',
						concreteTypes:
						[
							{name: 'value', alias: 'exchange', type: 'System.String'}
						]
					},
					{
						name: 'subscriberId',
						concreteTypes:
						[
							{name: 'value', alias: 'subscriberId', type: 'System.String'}
						]
					}
				]
			}");
		}

		public static Schema CreateAddressSchema()
		{
			return InstantiateSchema(@"
			{
				name: 'address', 
				subtypes:
				[
					{
						name: 'streetPrefix',
						concreteTypes:
						[
							{name: 'value', alias: 'streetPrefix', type: 'System.String'}
						]
					},
					{
						name: 'streetName',
						subtypes:
						[
							{
								name: 'name',
								concreteTypes:
								[
									{name: 'value', alias: 'streetName', type: 'System.String'}
								]
							}
						]
					},
					{
						name: 'streetPostfix',
						concreteTypes:
						[
							{name: 'value', alias: 'streetPostfix', type: 'System.String'}
						]
					},
					{
						name: 'boxNumber',
						concreteTypes:
						[
							{name: 'value', alias: 'boxNumber', type: 'System.String'}
						]
					},
					{
						name: 'city',
						concreteTypes:
						[
							{name: 'value', alias: 'city', type: 'System.String'}
						]
					},
					{
						name: 'state',
						subtypes:
						[
							{
								name: 'name',
								concreteTypes:
								[
									{name: 'name', alias: 'abbr', type: 'System.String'}
								]
							}
						]
					},
					{
						name: 'zipCode',
						concreteTypes:
						[
							{name: 'zip', type: 'System.String'},
							{name: 'zip4', type: 'System.String'}
						]
					},
				]
			}");
		}

		public static Schema CreateTimeSchema()
		{
			return InstantiateSchema(@"
			{
				name: 'time', 
				subTypes:
				[
					{
						name: 'hour24',
						concreteTypes:
						[
							{name: 'value', alias: 'hour24', type: 'System.Int32'}
						],
					},
					{
						name: 'minute',
						concreteTypes:
						[
							{name: 'value', alias: 'minute', type: 'System.Int32'}
						]
					},
					{
						name: 'second',
						concreteTypes:
						[
							{name: 'value', alias: 'second', type: 'System.Int32'}
						]
					}
				]
			}");
		}
	}
}
