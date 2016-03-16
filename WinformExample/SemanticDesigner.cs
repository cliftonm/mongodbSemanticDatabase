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
		protected Model model;

		protected CollectionView collectionView;
		protected SemanticView semanticView;
		protected AssociationView assocView;
		protected AssociatedDataView assocDataView;
		protected SemanticTreeView semanticTreeView;

		protected SemanticController semanticController;
		protected CollectionController collectionController;

		protected Schema assocSchema;
		protected Schema fromSchema;
		protected Schema toSchema;
		protected bool showForward = true;

		public SemanticDesigner(Model model)
		{
			this.model = model;
			InitializeComponent();

			new PlanView(tbPlan);
			new LogView(tbLog);
			assocDataView = new AssociatedDataView(lblAssociatedData, dgvAssociationData);
			assocView = new AssociationView(model, dgvAssociations);
			semanticView = new SemanticView(lblSemanticType, dgvSemanticData);
			semanticController = new SemanticController(model);
			semanticTreeView = new SemanticTreeView(model, tvTypes);

			// TODO: Put all local event handlers into the semanticTreeView or create a semanticTreeController.
			tvTypes.AfterSelect += semanticController.AfterSelectEvent;
			tvTypes.AfterSelect += OnSemanticTypeSelected;
			dgvSemanticData.SelectionChanged += semanticController.SelectionChangedEvent;

			collectionView = new CollectionView(lblCollectionName, dgvCollectionData);
			collectionController = new CollectionController(model);
			tvTypes.AfterSelect += collectionController.AfterSelectEvent;

			Program.serviceManager.Get<ISemanticProcessor>().Register<AssociationViewMembrane>(this);

			LoadSchema();
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Associations assoc)
		{
			// TODO: Implement reverse association button list.
			this.BeginInvoke(() =>
				{
					RemoveAssociationButtons();
					int n = CreateForwardAssociationButtons(assoc.ForwardSchemaNames);
					CreateReverseAssociationButtons(assoc.ReverseSchemaNames, n);
				});
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
			showForward = true;
			// TODO: If we make this a separate view class, it can maintain the from/to/assoc Schema variables itself instead of hold them in the main form class!
			string assocName = (string)((Button)sender).Tag;
			string fromSchemaName = assocName.LeftOf("_");
			string toSchemaName = assocName.RightOf("_");
			fromSchema = model.GetSchema(fromSchemaName);
			toSchema = model.GetSchema(toSchemaName);
			assocSchema = model.Db.GetAssociationSchema(fromSchema, toSchema);
			ShowForwardAssociatedData();
		}

		protected void OnShowReverseAssociatedData(object sender, EventArgs e)
		{
			showForward = false;
			// TODO: If we make this a separate view class, it can maintain the from/to/assoc Schema variables itself instead of hold them in the main form class!
			string assocName = (string)((Button)sender).Tag;
			string fromSchemaName = assocName.LeftOf("_");
			string toSchemaName = assocName.RightOf("_");
			fromSchema = model.GetSchema(fromSchemaName);
			toSchema = model.GetSchema(toSchemaName);
			assocSchema = model.Db.GetAssociationSchema(fromSchema, toSchema);
			ShowReverseAssociatedData();
		}

		protected void ShowForwardAssociatedData()
		{
			// TODO: Duplicate code, sort of (notice use of assocSchema and toSchema)
			BsonDocument filter = new BsonDocument(fromSchema.Name + "Id", new ObjectId(dgvSemanticData.SelectedRow()["_id"].ToString()));
			// List<BsonDocument> docs = model.Db.Query(assocSchema, filter);
			string plan;
			List<BsonDocument> docs = model.Db.QueryAssociationServerSide(fromSchema, toSchema, out plan, filter);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<PlanViewMembrane, ST_Plan>(p => p.Plan = plan);
			List<ConcreteType> semanticColumns;
			DataTable dt = DataHelpers.InitializeSemanticColumns(toSchema, out semanticColumns);
			DataHelpers.PopulateSemanticTable(dt, docs, toSchema);
			AddForwardAssociationNames(dt, docs);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_Data>(data => { data.Table = dt; data.Schema = toSchema; });
		}

		protected void ShowReverseAssociatedData()
		{
			// TODO: Duplicate code, sort of (notice use of assocSchema and fromSchema)
			BsonDocument filter = new BsonDocument(toSchema.Name + "Id", new ObjectId(dgvSemanticData.SelectedRow()["_id"].ToString()));
			// List<BsonDocument> docs = model.Db.Query(assocSchema, filter);
			string plan;
			List<BsonDocument> docs = model.Db.QueryAssociationServerSide(fromSchema, toSchema, out plan, filter);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<PlanViewMembrane, ST_Plan>(p => p.Plan = plan);
			List<ConcreteType> semanticColumns;
			DataTable dt = DataHelpers.InitializeSemanticColumns(fromSchema, out semanticColumns);
			DataHelpers.PopulateSemanticTable(dt, docs, fromSchema);
			AddForwardAssociationNames(dt, docs);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_Data>(data => { data.Table = dt; data.Schema = fromSchema; });
		}

		protected void AddForwardAssociationNames(DataTable dt, List<BsonDocument> docs)
		{
			dt.Columns.Add("fwdAssocName");

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i]["fwdAssocName"] = docs[i]["fwdAssocName"];
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
