using System;
using System.Collections.Generic;
using System.Data;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public static class DataHelpers
	{
		/// <summary>
		/// Recurses into subtypes to create a flat view of the semantic hierarchy.
		/// </summary>
		public static DataTable InitializeSemanticColumns(Schema schema, out List<ConcreteType> semanticColumns)
		{
			DataTable dt = new DataTable();
			dt.TableName = schema.Name;
			semanticColumns = new List<ConcreteType>();
			dt.Columns.Add("_id");
			PopulateSemanticColumns(dt, schema, semanticColumns);

			return dt;
		}

		public static void PopulateSemanticColumns(DataTable dt, Schema schema, List<ConcreteType> columns)
		{
			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				dt.Columns.Add(ct.Alias);
				columns.Add(ct);
			}

			foreach (Schema st in schema.Subtypes)
			{
				PopulateSemanticColumns(dt, st, columns);
			}
		}

		/// <summary>
		/// Recurse into subtypes to populate all concrete data.
		/// </summary>
		public static void PopulateSemanticTable(DataTable dt, List<BsonDocument> docs, Schema schema)
		{
			foreach (BsonDocument doc in docs)
			{
				DataRow row = dt.NewRow();

				if (doc.Contains("_id"))
				{
					row["_id"] = doc["_id"];
				}

				PopulateConcreteTypes(row, doc, schema);
				dt.Rows.Add(row);
			}
		}

		public static void PopulateConcreteTypes(DataRow row, BsonDocument doc, Schema schema)
		{
			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				if (doc.Contains(ct.Alias))
				{
					row[ct.Alias] = doc[ct.Alias];
				}
				else if (doc.Contains(ct.Name))
				{
					row[ct.Alias] = doc[ct.Name];
				}
				else
				{
					row[ct.Alias] = " --- ";
				}
			}

			foreach (Schema st in schema.Subtypes)
			{
				PopulateConcreteTypes(row, doc, st);
			}
		}


	}
}
