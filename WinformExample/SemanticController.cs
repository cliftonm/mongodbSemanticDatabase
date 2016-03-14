using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class SemanticController
	{
		protected Model model;
		protected List<ConcreteType> semanticColumns;
		protected Dictionary<string, string> currentValues;
		protected Schema currentSchema;
		protected int currentRowIdx = -1;
		protected string currentId;
		protected bool ignoreUpdate;
		protected DataRow newRow;

		public SemanticController(Model model)
		{
			this.model = model;
			currentValues = new Dictionary<string, string>();
			semanticColumns = new List<ConcreteType>();
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
						string plan = model.Db.ShowPlan(currentSchema);
						Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<PlanViewMembrane, ST_Plan>(p => p.Plan = plan);
						ResetBuffers();
						ShowSemanticData(currentSchema);
					}
					else if (item is ConcreteType)
					{
					}
				});
		}

		public void NewRowEvent(object sender, DataTableNewRowEventArgs e)
		{
			Helpers.Log("New Row");
			newRow = e.Row;
		}

		public void RowDeletedEvent(object sender, DataRowChangeEventArgs e)
		{
			Helpers.Log("Row Deleted");
		}

		public void RowChangedEvent(object sender, DataRowChangeEventArgs e)
		{
			if (!ignoreUpdate)
			{
				Helpers.Try(() =>
					{
						Helpers.Log("Row Changed");

						if (newRow != null)
						{
							Helpers.Log("Inserting Record");
							BsonDocument doc = GetDocument(newRow);
							Helpers.Log(doc.ToString());
							string id = model.Db.Insert(currentSchema, doc);
							ignoreUpdate = true;			// updating the _id field causes a RowChanged event!
							newRow["_id"] = id;
							ignoreUpdate = false;
							currentId = id;
							newRow = null;
						}
						else
						{
							Helpers.Log("Updating Record");
							BsonDocument docOld = GetDocument(currentValues);
							BsonDocument docNew = GetDocument(Program.serviceManager.Get<ISemanticViewService>().SelectedRow);
							Helpers.Log("  from: " + docOld.ToString());
							Helpers.Log("  to: " + docNew.ToString());
							string id = model.Db.Update(currentSchema, docOld, docNew, currentId);
							ignoreUpdate = true;			// updating the _id field causes a RowChanged event!
							Program.serviceManager.Get<ISemanticViewService>().SelectedRow["_id"] = id;
							ignoreUpdate = false;
							currentId = id;
						}
					});
			}
		}

		/// <summary>
		/// User is navigating the semantic grid.
		/// </summary>
		public void SelectionChangedEvent(object sender, EventArgs e)
		{
			Helpers.Try(() =>
				{
					ISemanticViewService svc = Program.serviceManager.Get<ISemanticViewService>();
					int rowIdx = svc.SelectedRowIndex;

					if (rowIdx != currentRowIdx)
					{
						Helpers.Log("Selection Changed: " + rowIdx);
						currentRowIdx = rowIdx;

						if (rowIdx >= 0)
						{
							if (rowIdx < svc.NumRows)
							{
								if (newRow != null)
								{
									Helpers.Log("Adding row cancelled");
									newRow = null;
								}

								CaptureCurrentValues(svc.GetRowAt(rowIdx));
							}
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

		protected void ShowSemanticData(Schema schema)
		{
			Helpers.Try(() => 
				{
					List<BsonDocument> docs = model.Db.Query(schema);
					DataTable dt = InitializeSemanticColumns(schema);
					PopulateSemanticTable(dt, docs, schema);
					Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<SemanticViewMembrane, ST_Data>(data => data.Table = dt);
					dt.RowChanged += RowChangedEvent;
					dt.TableNewRow += NewRowEvent;
					dt.RowDeleted += RowDeletedEvent;
				});
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
