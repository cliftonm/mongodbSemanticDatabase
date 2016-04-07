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
		public Schema Schema { get { return schema; } }
		public int Records { get { return dataView.Count; } }

		protected Label label;
		protected DataGridView view;
		protected Model model;
		protected Schema schema;
		protected DataView dataView;
		protected SemanticDesigner mainView;

		public AssociationView(Model model, DataGridView view, SemanticDesigner mainView)
		{
			this.model = model;
			this.view = view;
			this.mainView = mainView;
			// Program.serviceManager.Get<ISemanticProcessor>().Register<AssociationViewMembrane>(this);
			view.SelectionChanged += OnSelectionChanged;
		}

		public void Update(Schema schema)
		{
			this.schema = schema;
			string fwdName = schema.Name + "_";
			string revName = "_" + schema.Name;
			List<string> collections = model.Db.GetCollections();
			List<string> forwardAssoc = collections.Where(n => n.BeginsWith(fwdName) && !n.EndsWith("Association")).ToList();
			List<string> reverseAssoc = collections.Where(n => n.EndsWith(revName)).ToList();

			// Update forward and reverse navigation buttons.
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociationViewMembrane, ST_Associations>(data => 
			{
				data.ForwardSchemaNames = forwardAssoc;
				data.ReverseSchemaNames = reverseAssoc;
			});

			List<string> fullList = new List<string>();
			fullList.AddRange(forwardAssoc);
			fullList.AddRange(reverseAssoc);

			DataTable dt = Helpers.FillTable(fullList, "Associations");
			dataView = new DataView(dt);
			view.DataSource = dataView;
		}

		public string GetAssociationSchemaName()
		{
			return view.SelectedRow()[0].ToString();
		}

		protected void OnSelectionChanged(object sender, EventArgs e)
		{
			if (view.HasSelectedRow())
			{
				mainView.DisableMoveUp();
				string associations = view.SelectedRow()[0].ToString();
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

				Schema withSchema = model.GetSchema(withSchemaName);

				if (withSchema == null)
				{
					Helpers.Log("Schema " + withSchemaName + " must be fully defined for now and cannot be a 'discovered' schema.");
				}
				else
				{
					// TODO: Duplicate code.
					List<BsonDocument> docs = model.Db.Query(withSchema);
					List<ConcreteType> semanticColumns;
					DataTable dt = DataHelpers.InitializeSemanticColumns(withSchema, out semanticColumns);
					DataHelpers.PopulateSemanticTable(dt, docs, withSchema);
					Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_Data>(data => { data.Table = dt; data.Schema = withSchema; });
				}
			}
		}
	}
}
