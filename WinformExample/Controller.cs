using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class Controller
	{
		public SemanticDesigner View { get; set; }

		protected Model model;

		public Controller(Model model)
		{
			this.model = model;
		}

		public void InstantiateMissingLookups()
		{
			if (!model.Db.GetCollections().Contains("monthLookup"))
			{
				InstantiateMonthLookup(model.Db, model.GetSchema("monthLookup"));	
			}
		}

		public void AfterSelectEvent(object sender, TreeViewEventArgs e)
		{
			object item = e.Node.Tag;

			if (item is Schema)
			{
				ShowCollection((Schema)item);
				ShowSemanticData((Schema)item);
			}
			else if (item is ConcreteType)
			{
			}
		}

		protected void ShowCollection(Schema schema)
		{
			List<BsonDocument> docs = model.Db.GetAll(schema.Name);
			DataTable dt = InitializeCollectionColumns(schema);
			PopulateCollectionTable(dt, docs, schema);
			View.ShowCollectionData(dt);
		}

		protected void ShowSemanticData(Schema schema)
		{
			List<BsonDocument> docs = model.Db.Query(schema);
			DataTable dt = InitializeSemanticColumns(schema);
			PopulateSemanticTable(dt, docs, schema);
			View.ShowSemanticData(dt);
		}

		/// <summary>
		/// Initializes the columns of a table with the concrete types for collection associated with the schema,
		/// therefore no sub-types are parsed.
		/// </summary>
		protected DataTable InitializeCollectionColumns(Schema schema)
		{
			DataTable dt = new DataTable();
			dt.TableName = schema.Name;

			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				dt.Columns.Add(ct.Name);
			}

			return dt;
		}

		/// <summary>
		/// Recurses into subtypes to create a flat view of the semantic hierarchy.
		/// </summary>
		protected DataTable InitializeSemanticColumns(Schema schema)
		{
			DataTable dt = new DataTable();
			dt.TableName = schema.Name;
			PopulateSemanticColumns(dt, schema);

			return dt;
		}

		protected void PopulateSemanticColumns(DataTable dt, Schema schema)
		{
			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				dt.Columns.Add(ct.Alias);
			}

			foreach (Schema st in schema.Subtypes)
			{
				PopulateSemanticColumns(dt, st);
			}
		}

		protected void PopulateCollectionTable(DataTable dt, List<BsonDocument> docs, Schema schema)
		{
			foreach (BsonDocument doc in docs)
			{
				DataRow row = dt.NewRow();

				foreach (ConcreteType ct in schema.ConcreteTypes)
				{
					row[ct.Name] = doc[ct.Name];
				}

				dt.Rows.Add(row);
			}
		}

		/// <summary>
		/// Recurse into subtypes to populate all concrete data.
		/// </summary>
		protected void PopulateSemanticTable(DataTable dt, List<BsonDocument> docs, Schema schema)
		{
			foreach (BsonDocument doc in docs)
			{
				DataRow row = dt.NewRow();
				PopulateConcreteTypes(row, doc, schema);
				dt.Rows.Add(row);
			}
		}

		protected void PopulateConcreteTypes(DataRow row, BsonDocument doc, Schema schema)
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

		protected void InstantiateMonthLookup(SemanticDatabase sd, Schema schema)
		{
			sd.Insert(schema, BsonDocument.Parse("{month: 1, monthName: 'January', monthAbbr: 'Jan'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 2, monthName: 'February', monthAbbr: 'Feb'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 3, monthName: 'March', monthAbbr: 'Mar'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 4, monthName: 'April', monthAbbr: 'Apr'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 5, monthName: 'May', monthAbbr: 'May'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 6, monthName: 'June', monthAbbr: 'Jun'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 7, monthName: 'July', monthAbbr: 'Jul'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 8, monthName: 'August', monthAbbr: 'Aug'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 9, monthName: 'September', monthAbbr: 'Sep'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 10, monthName: 'October', monthAbbr: 'Oct'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 11, monthName: 'November', monthAbbr: 'Nov'}"));
			sd.Insert(schema, BsonDocument.Parse("{month: 12, monthName: 'December', monthAbbr: 'Dec'}"));
		}
	}
}
