using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MongoDB.Bson;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public partial class SemanticDesigner : Form, IReceptor
	{
		public AssociationView AssociationView { get { return assocView; } }

		protected Model model;

		protected CollectionView collectionView;
		protected SemanticView semanticView;
		protected AssociationView assocView;
		protected AssociatedDataView assocDataView;
		protected SemanticTreeView semanticTreeView;

		protected SemanticController semanticController;
		protected CollectionController collectionController;

		protected Schema assocSchema;
		protected bool showForward = true;

		public SemanticDesigner(Model model)
		{
			this.model = model;
			InitializeComponent();

			new PlanView(tbPlan);
			new LogView(tbLog);
			assocDataView = new AssociatedDataView(model, lblAssociatedData, dgvAssociationData);
			assocView = new AssociationView(model, dgvAssociations, this);
			semanticView = new SemanticView(lblSemanticType, dgvSemanticData);
			semanticController = new SemanticController(model, this);
			semanticTreeView = new SemanticTreeView(model, tvTypes);

			// TODO: Put all local event handlers into the semanticTreeView or create a semanticTreeController.
			tvTypes.AfterSelect += semanticController.AfterSelectEvent;
			tvTypes.AfterSelect += OnSemanticTypeSelected;
			dgvSemanticData.SelectionChanged += semanticController.SelectionChangedEvent;
			btnMainView.Click += semanticController.MoveToMainView;

			collectionView = new CollectionView(lblCollectionName, dgvCollectionData);
			collectionController = new CollectionController(model);
			tvTypes.AfterSelect += collectionController.AfterSelectEvent;

			Program.serviceManager.Get<ISemanticProcessor>().Register<AssociationViewMembrane>(this);

			LoadSchema();
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Associations assoc)
		{
			this.BeginInvoke(() =>
				{
					RemoveAssociationButtons();
					int n = CreateForwardAssociationButtons(assoc.ForwardSchemaNames);
					CreateReverseAssociationButtons(assoc.ReverseSchemaNames, n);
				});
		}

		public void EnableMoveUp()
		{
			btnMainView.Enabled = true;
		}

		public void DisableMoveUp()
		{
			btnMainView.Enabled = false;
		}

		protected void RemoveAssociationButtons()
		{
			gbNavigate.Controls.Clear();
		}

		protected int CreateForwardAssociationButtons(List<string> names, int n = 0)
		{
			List<string> assocWith = names.OrderBy(name => name.RightOf("_")).ToList();

			foreach (string name in assocWith)
			{
				Button btn = new Button();
				btn.Location = new Point(10, 15 + n * 30);
				btn.Size = new Size(gbNavigate.Width - 20, 25);
				btn.Text = name.RightOf("_");
				btn.Tag = name;
				btn.Click += OnShowForwardAssociatedData;
				gbNavigate.Controls.Add(btn);
				++n;
			}

			return n;
		}

		protected int CreateReverseAssociationButtons(List<string> names, int n = 0)
		{
			List<string> assocWith = names.OrderBy(name => name.LeftOf("_")).ToList();

			foreach (string name in assocWith)
			{
				Button btn = new Button();
				btn.Location = new Point(10, 15 + n * 30);
				btn.Size = new Size(gbNavigate.Width - 20, 25);
				btn.Text = name.LeftOf("_");
				btn.Tag = name;
				btn.Click += OnShowReverseAssociatedData;
				gbNavigate.Controls.Add(btn);
				++n;
			}

			return n;
		}

		protected void OnShowForwardAssociatedData(object sender, EventArgs e)
		{
			try
			{
				showForward = true;
				// TODO: If we make this a separate view class, it can maintain the from/to/assoc Schema variables itself instead of hold them in the main form class!
				string assocName = (string)((Button)sender).Tag;
				string fromSchemaName = assocName.LeftOf("_");
				string toSchemaName = assocName.RightOf("_");
				model.FromSchema = model.GetSchema(fromSchemaName);
				model.ToSchema = model.GetSchema(toSchemaName);
				model.IsReverseAssociation = false;
				assocSchema = model.Db.GetAssociationSchema(model.FromSchema, model.ToSchema);
				ShowForwardAssociatedData();
				EnableMoveUp();
			}
			catch (Exception ex)
			{
				Helpers.Log(ex.Message);
				ClearAssociationData();
			}
		}

		protected void OnShowReverseAssociatedData(object sender, EventArgs e)
		{
			try
			{
				showForward = false;
				// TODO: If we make this a separate view class, it can maintain the from/to/assoc Schema variables itself instead of hold them in the main form class!
				string assocName = (string)((Button)sender).Tag;
				string fromSchemaName = assocName.LeftOf("_");
				string toSchemaName = assocName.RightOf("_");
				model.FromSchema = model.GetSchema(fromSchemaName);
				model.ToSchema = model.GetSchema(toSchemaName);
				model.IsReverseAssociation = true;
				assocSchema = model.Db.GetAssociationSchema(model.FromSchema, model.ToSchema);
				ShowReverseAssociatedData();
				EnableMoveUp();
			}
			catch (Exception ex)
			{
				Helpers.Log(ex.Message);
				ClearAssociationData();
			}
		}

		protected void ClearAssociationData()
		{
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_NoData>();
		}

		protected void ShowForwardAssociatedData()
		{
			// TODO: Duplicate code, sort of (notice use of assocSchema and toSchema)
			BsonDocument filter = new BsonDocument(model.FromSchema.Name + "Id", new ObjectId(dgvSemanticData.SelectedRow()["_id"].ToString()));
			// List<BsonDocument> docs = model.Db.Query(assocSchema, filter);
			string plan;
			List<BsonDocument> docs = model.Db.QueryAssociationServerSide(model.FromSchema, model.ToSchema, out plan, filter);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<PlanViewMembrane, ST_Plan>(p => p.Plan = plan);
			List<ConcreteType> semanticColumns;
			DataTable dt = DataHelpers.InitializeSemanticColumns(model.ToSchema, out semanticColumns);
			DataHelpers.PopulateSemanticTable(dt, docs, model.ToSchema);
			AddForwardAssociationNames(dt, docs);
			AddSchemaFromToIds(dt, docs, model.FromSchema, model.ToSchema);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_AssociatedData>(data => { data.Table = dt; data.Schema = model.ToSchema; });
		}

		protected void ShowReverseAssociatedData()
		{
			// TODO: Duplicate code, sort of (notice use of assocSchema and fromSchema)
			BsonDocument filter = new BsonDocument(model.ToSchema.Name + "Id", new ObjectId(dgvSemanticData.SelectedRow()["_id"].ToString()));
			// List<BsonDocument> docs = model.Db.Query(assocSchema, filter);
			string plan;
			List<BsonDocument> docs = model.Db.QueryAssociationServerSide(model.FromSchema, model.ToSchema, out plan, filter);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<PlanViewMembrane, ST_Plan>(p => p.Plan = plan);
			List<ConcreteType> semanticColumns;
			DataTable dt = DataHelpers.InitializeSemanticColumns(model.FromSchema, out semanticColumns);
			DataHelpers.PopulateSemanticTable(dt, docs, model.FromSchema);
			AddReverseAssociationNames(dt, docs);
			AddSchemaFromToIds(dt, docs, model.FromSchema, model.ToSchema);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_AssociatedData>(data => { data.Table = dt; data.Schema = model.FromSchema; });
		}

		protected void AddForwardAssociationNames(DataTable dt, List<BsonDocument> docs)
		{
			dt.Columns.Add("fwdAssocName");

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i]["fwdAssocName"] = docs[i]["fwdAssocName"];
			}
		}

		protected void AddReverseAssociationNames(DataTable dt, List<BsonDocument> docs)
		{
			dt.Columns.Add("revAssocName");

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i]["revAssocName"] = docs[i]["revAssocName"];
			}
		}

		/// <summary>
		/// Add ID's of the source from/to records so we can use the "to" ID's for filtering records when the user moves schema to the main schema view for further navigation.
		/// </summary>
		protected void AddSchemaFromToIds(DataTable dt, List<BsonDocument> docs, Schema fromSchema, Schema toSchema)
		{
			string fromSchemaId = fromSchema.Name + "Id";
			string toSchemaId = model.ToSchema.Name + "Id";
			dt.Columns.Add(fromSchemaId);
			dt.Columns.Add(toSchemaId);

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i][fromSchemaId] = docs[i][fromSchemaId];
				dt.Rows[i][toSchemaId] = docs[i][toSchemaId];
			}
		}

		protected void OnSemanticTypeSelected(object sender, TreeViewEventArgs e)
		{
			object item = e.Node.Tag;

			if (item is Schema)
			{
				assocView.Update((Schema)item);

				if (assocView.Records == 0)
				{
					assocDataView.Clear();
				}
			}
		}

		protected void InitializeSchemaView()
		{
			TreeNode root = tvTypes.Nodes.Add("Semantic Schema");

			foreach (Schema schema in model.Schemata.OrderBy(s => s.Name))
			{
				root.Nodes.Add(CreateSchemaNode(schema));
			}
		}

		protected TreeNode CreateSchemaNode(Schema schema)
		{
			TreeNode schemaRootNode = new TreeNode(Helpers.GetSchemaNodeText(schema));
			schemaRootNode.Tag = schema;
			TreeNode concreteTypesNode = schemaRootNode.Nodes.Add("Concrete Types");
			TreeNode subTypesNode = schemaRootNode.Nodes.Add("Subtypes");

			AddConcreteTypes(schema, concreteTypesNode);
			AddSubtypes(schema, subTypesNode);

			return schemaRootNode;
		}

		protected void AddConcreteTypes(Schema schema, TreeNode cttn)
		{
			foreach (ConcreteType ct in schema.ConcreteTypes)
			{
				TreeNode ctNode = cttn.Nodes.Add(Helpers.GetConcreteTypeText(ct));
				ctNode.Tag = ct;
			}
		}

		protected void AddSubtypes(Schema schema, TreeNode subTypesNode)
		{
			foreach(Schema st in schema.Subtypes)
			{
				TreeNode tn = CreateSchemaNode(st);
				subTypesNode.Nodes.Add(tn);
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			Schema fromSchema = tvTypes.SelectedNode.Tag as Schema;

			if (fromSchema != null)
			{
				Form form = new CreateAssociationDlg(model, fromSchema);
				form.ShowDialog();
				assocView.Update(fromSchema);
			}
			else
			{
				MessageBox.Show("Please select a 'from' collection.", "To Do...", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		// TODO: Need to handle reverse associations.
		private void btnAssociateRecords_Click(object sender, EventArgs e)
		{
			if (semanticView.HasSelectedRow && assocDataView.HasSelectedRow)
			{
				DataRow fromRow = semanticView.SelectedRow;
				DataRow toRow = assocDataView.SelectedRow;
				string fromId = fromRow["_id"].ToString();
				string toId = toRow["_id"].ToString();
				Schema fromSchema = semanticView.Schema;
				Schema toSchema = assocDataView.Schema;
				Schema assocSchema = model.Db.GetAssociationSchema(fromSchema, toSchema);

				string fwdName = fromSchema.Name;
				string revName = toSchema.Name;
				string fwdDescr = lblFwdAssoc.Text;
				string revDescr = tbRevAssoc.Text;

				BsonDocument doc = new BsonDocument(fwdName + "Id", new ObjectId(fromId));
				doc.Add(revName + "Id", new ObjectId(toId));
				doc.Add("forwardAssociationName", tbFwdAssoc.Text);
				doc.Add("reverseAssociationName", tbRevAssoc.Text);
				model.Db.Insert(assocSchema, doc);
			}
		}

		private void btnDeleteAssoc_Click(object sender, EventArgs e)
		{
			if (dgvAssociationData.HasSelectedRow())
			{
				DataRow row = dgvAssociationData.SelectedRow();
				string id = row["_id"].ToString();
				model.Db.DeleteAssociation(assocSchema, new BsonDocument("_id", new ObjectId(id)));
				showForward.IfElse(()=>ShowForwardAssociatedData(), ()=>ShowReverseAssociatedData());
			}
		}

		private void mnuLoadSchema_Click(object sender, EventArgs e)
		{
			LoadSchema();
		}

		protected void LoadSchema()
		{
			model.Load("schema.json");
			tvTypes.Nodes.Clear();
			InitializeSchemaView();

			if (tvTypes.Nodes.Count > 0)
			{
				tvTypes.Nodes[0].Expand();
			}
		}

		private void mnuSaveSchema_Click(object sender, EventArgs e)
		{
			model.Save("schema.json");
		}

		private void mnuExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void mnuImportSchema_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Coming soon!", "Not implemented.", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
