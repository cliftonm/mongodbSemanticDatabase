using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class AssociationView : IReceptor
	{
		// TODO: We keep repeating these helper!
		public bool HasSelectedRow { get { return view.SelectedRows.Count != 0; } }
		public DataRow SelectedRow { get { return ((DataView)view.DataSource)[view.SelectedRows[0].Index].Row; } }

		public int SelectedRowIndex { get { return HasSelectedRow ? view.SelectedRows[0].Index : -2; } }
		public int NumRows { get { return ((DataView)view.DataSource).Table.Rows.Count; } }

		protected Label label;
		protected DataGridView view;
		protected Model model;
		protected List<ConcreteType> semanticColumns;
		protected Schema schema;
		protected DataView dataView;

		public AssociationView(Model model, DataGridView view)
		{
			this.model = model;
			this.view = view;
			// Program.serviceManager.Get<ISemanticProcessor>().Register<AssociationViewMembrane>(this);
			view.SelectionChanged += OnSelectionChanged;
			semanticColumns = new List<ConcreteType>();
		}

		public void Update(Schema schema)
		{
			this.schema = schema;
			string fwdName = schema.Name + "_";
			string revName = "_" + schema.Name;
			List<string> collections = model.Db.GetCollections();
			List<string> forwardAssoc = collections.Where(n => n.BeginsWith(fwdName) && !n.EndsWith("Association")).ToList();
			List<string> reverseAssoc = collections.Where(n => n.EndsWith(revName)).ToList();

			List<string> fullList = new List<string>();
			fullList.AddRange(forwardAssoc);
			fullList.AddRange(reverseAssoc);

			DataTable dt = Helpers.FillTable(fullList, "Associations");
			dataView = new DataView(dt);
			view.DataSource = dataView;
		}

		protected void OnSelectionChanged(object sender, EventArgs e)
		{
			if (HasSelectedRow)
			{
				string associations = SelectedRow[0].ToString();
				string withSchemaName;

				// Forward or reverse?
				if (associations.BeginsWith(schema.Name))
				{
					withSchemaName = associations.RightOf("_");
				}
				else
				{
					withSchemaName = associations.LeftOf("_");
				}

				Schema withSchema = model.Schemata.FirstOrDefault(s => s.Name == withSchemaName);

				if (withSchema == null)
				{
					Helpers.Log("Schema " + withSchemaName + " must be fully defined for now and cannot be a 'discovered' schema.");
				}
				else
				{
					// TODO: Duplicate code.
					List<BsonDocument> docs = model.Db.Query(withSchema);
					DataTable dt = InitializeSemanticColumns(withSchema);
					PopulateSemanticTable(dt, docs, withSchema);
					Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_Data>(data => data.Table = dt);
				}
			}
		}

		// TODO: All this is exactly the same in SemanticController.cs

		/// <summary>
		/// Recurses into subtypes to create a flat view of the semantic hierarchy.
		/// </summary>
		protected DataTable InitializeSemanticColumns(Schema schema)
		{
			DataTable dt = new DataTable();
			dt.TableName = schema.Name;
			semanticColumns.Clear();
			dt.Columns.Add("_id");
			PopulateSemanticColumns(dt, schema, semanticColumns);

			return dt;
		}

		protected void PopulateSemanticColumns(DataTable dt, Schema schema, List<ConcreteType> columns)
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
		protected void PopulateSemanticTable(DataTable dt, List<BsonDocument> docs, Schema schema)
		{
			foreach (BsonDocument doc in docs)
			{
				DataRow row = dt.NewRow();
				row["_id"] = doc["_id"];
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
	}
}
