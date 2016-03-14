using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class CollectionController
	{
		protected Schema currentSchema;
		protected Model model;

		public CollectionController(Model model)
		{
			this.model = model;
		}

		/// <summary>
		/// Treeview selection.
		/// </summary>
		public void AfterSelectEvent(object sender, TreeViewEventArgs e)
		{
			Helpers.Try(() =>
			{
				object item = e.Node.Tag;

				if (item is Schema)
				{
					currentSchema = (Schema)item;
					ShowCollection(currentSchema);
				}
				else if (item is ConcreteType)
				{
				}
			});
		}

		protected void ShowCollection(Schema schema)
		{
			Helpers.Try(() =>
			{
				List<BsonDocument> docs = model.Db.GetAll(schema.Name);
				DataTable dt = InitializeCollectionColumns(schema);
				PopulateCollectionTable(dt, docs, schema);
				Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<CollectionViewMembrane, ST_Data>(data => data.Table = dt);
			});
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

	}
}
