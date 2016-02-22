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
		protected List<ConcreteType> semanticColumns;
		protected Dictionary<string, string> currentValues;
		protected Schema currentSchema;
		public string currentId;
		protected DataRow newRow;

		public Controller(Model model)
		{
			this.model = model;
			currentValues = new Dictionary<string, string>();
			semanticColumns = new List<ConcreteType>();
		}

		public void InstantiateMissingLookups()
		{
			Try(() =>
				{
					if (!model.Db.GetCollections().Contains("monthLookup"))
					{
						InstantiateMonthLookup(model.Db, model.GetSchema("monthLookup"));
					}
				});
		}

		/// <summary>
		/// Treeview selection.
		/// </summary>
		public void AfterSelectEvent(object sender, TreeViewEventArgs e)
		{
			Try(() =>
				{
					object item = e.Node.Tag;

					if (item is Schema)
					{
						ResetBuffers();
						currentSchema = (Schema)item;
						ShowCollection(currentSchema);
						ShowSemanticData(currentSchema);
					}
					else if (item is ConcreteType)
					{
					}
				});
		}

		public void NewRowEvent(object sender, DataTableNewRowEventArgs e)
		{
			View.Log("New Row");
			newRow = e.Row;
		}

		public void RowDeletedEvent(object sender, DataRowChangeEventArgs e)
		{
			View.Log("Row Deleted");
		}

		public void RowChangedEvent(object sender, DataRowChangeEventArgs e)
		{
			Try(() =>
				{
					View.Log("Row Changed");

					if (newRow != null)
					{
						View.Log("Inserting Record");
						BsonDocument doc = GetDocument(newRow);
						View.Log(doc.ToString());
						model.Db.Insert(currentSchema, doc);
						newRow = null;
					}
					else
					{
						View.Log("Updating Record");
						BsonDocument docOld = GetDocument(currentValues);
						BsonDocument docNew = GetDocument(View.SelectedSemanticRow);
						View.Log("  from: " + docOld.ToString());
						View.Log("  to: " + docNew.ToString());
						model.Db.Update(currentSchema, docOld, docNew, currentId);
					}
				});
		}

		/// <summary>
		/// User is navigating the semantic grid.
		/// </summary>
		public void SelectionChangedEvent(object sender, EventArgs e)
		{
			Try(() =>
				{
					int rowIdx = View.SelectedSemanticRowIndex;
					View.Log("Selection Changed: " + rowIdx);

					if (rowIdx >= 0)
					{
						if (rowIdx < View.NumSemanticRows)
						{
							if (newRow != null)
							{
								View.Log("Adding row cancelled");
								newRow = null;
							}

							CaptureCurrentValues(View.GetSemanticRowAt(rowIdx));
						}
					}
				});
		}

		protected void ResetBuffers()
		{
			semanticColumns.Clear();
			currentValues.Clear();
			currentId = null;
		}

		protected BsonDocument GetDocument(DataRow row)
		{
			BsonDocument doc = new BsonDocument();

			foreach (ConcreteType ct in semanticColumns)
			{
				doc.Add(ct.Alias, row[ct.Alias].ToString());
			}

			return doc;
		}

		protected BsonDocument GetDocument(Dictionary<string, string> vals)
		{
			BsonDocument doc = new BsonDocument();

			foreach (ConcreteType ct in semanticColumns)
			{
				doc.Add(ct.Alias, currentValues[ct.Alias]);
			}

			return doc;
		}

		protected bool IsNewRow(DataGridViewRow row)
		{
			return row.Index == ((DataView)row.DataGridView.DataSource).Count;
		}

		protected void CaptureCurrentValues(DataRow row)
		{
			currentValues.Clear();
			currentId = row["_id"].ToString();

			foreach (ConcreteType ct in semanticColumns)
			{
				currentValues[ct.Alias] = row[ct.Alias].ToString();
			}
		}

		protected void ShowCollection(Schema schema)
		{
			Try(() =>
				{
					List<BsonDocument> docs = model.Db.GetAll(schema.Name);
					DataTable dt = InitializeCollectionColumns(schema);
					PopulateCollectionTable(dt, docs, schema);
					View.ShowCollectionData(dt);
				});
		}

		protected void ShowSemanticData(Schema schema)
		{
			Try(() => 
				{
					List<BsonDocument> docs = model.Db.Query(schema);
					DataTable dt = InitializeSemanticColumns(schema);
					PopulateSemanticTable(dt, docs, schema);
					View.ShowSemanticData(dt);
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

		protected void Try(Action activity)
		{
			try
			{
				activity();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
