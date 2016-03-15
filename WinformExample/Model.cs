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

		public void OpenDatabase(string dbName)
		{
			Db = new SemanticDatabase();
			Db.Open(dbName);
		}

		public void InitializeCoreSchemata()
		{
			Schemata = new List<Schema>();
			Schemata.Add(CoreSchemata.CreateDateSchema());
			Schemata.Add(CoreSchemata.CreateTimeSchema());
			Schemata.Add(CoreSchemata.CreateMonthNameLookupSchema());
			Schemata.Add(CoreSchemata.CreateAddressSchema());
			Schemata.Add(CoreSchemata.CreatePersonSchema());
			Schemata.Add(CoreSchemata.CreatePhoneNumberSchema());
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
