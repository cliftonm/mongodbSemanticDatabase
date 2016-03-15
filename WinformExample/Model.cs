using System;
using System.Collections.Generic;
using System.Linq;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class Model
	{
		public List<Schema> Schemata { get; protected set; }
		public SemanticDatabase Db { get; protected set; }

		public Model()
		{
			Db = new SemanticDatabase();
		}

		public void OpenDatabase(string dbName)
		{
			Db.Open(dbName);
		}

		public void InitializeCoreSchemata()
		{
			Schema personSchema;
			Schema dateSchema;
			Schema addressSchema;
			Schema phoneNumberSchema;
			Schemata = new List<Schema>();
			Schemata.Add(dateSchema = CoreSchemata.CreateDateSchema());
			Schemata.Add(CoreSchemata.CreateTimeSchema());
			Schemata.Add(CoreSchemata.CreateMonthNameLookupSchema());
			Schemata.Add(addressSchema = CoreSchemata.CreateAddressSchema());
			Schemata.Add(personSchema = CoreSchemata.CreatePersonSchema());
			Schemata.Add(phoneNumberSchema = CoreSchemata.CreatePhoneNumberSchema());

			// Examples for viewing association data.
			//Schemata.Add(Db.GetAssociationSchema(personSchema, dateSchema));
			//Schemata.Add(Db.GetAssociationSchema(personSchema, addressSchema));
			//Schemata.Add(Db.GetAssociationSchema(personSchema, phoneNumberSchema));
		}

		/// <summary>
		/// Returns null if schema by name is not found.
		/// </summary>
		public Schema GetSchema(string schemaName)
		{
			return Schemata.FirstOrDefault(s => s.Name == schemaName);
		}
	}
}
