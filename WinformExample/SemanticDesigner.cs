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

		protected SemanticController semanticController;
		protected CollectionController collectionController;

		public SemanticDesigner(Model model)
		{
			this.model = model;
			InitializeComponent();
			InitializeSchemaView();
			// tvTypes.ExpandAll();
			tvTypes.Nodes[0].Expand();

			new PlanView(tbPlan);
			new LogView(tbLog);
			assocDataView = new AssociatedDataView(lblAssociatedData, dgvAssociationData);
			assocView = new AssociationView(model, dgvAssociations);
			semanticView = new SemanticView(lblSemanticType, dgvSemanticData);
			semanticController = new SemanticController(model);

			tvTypes.AfterSelect += semanticController.AfterSelectEvent;
			tvTypes.AfterSelect += OnSemanticTypeSelected;
			dgvSemanticData.SelectionChanged += semanticController.SelectionChangedEvent;

			collectionView = new CollectionView(lblCollectionName, dgvCollectionData);
			collectionController = new CollectionController(model);
			tvTypes.AfterSelect += collectionController.AfterSelectEvent;

			Program.serviceManager.Get<ISemanticProcessor>().Register<AssociationViewMembrane>(this);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Associations assoc)
		{
			// TODO: Implement reverse association button list.
			this.BeginInvoke(() =>
				{
					RemoveAssociationButtons();
					CreateAssociationButtons(assoc.ForwardSchemaNames);
				});
		}

		protected void RemoveAssociationButtons()
		{
			gbNavigate.Controls.Clear();
		}

		protected void CreateAssociationButtons(List<string> names)
		{
			int n = 0;
			List<string> assocWith = names.Select(name => name.RightOf("_")).OrderBy(name => name).ToList();

			foreach (string name in assocWith)
			{
				Button btn = new Button();
				btn.Location = new Point(10, 15 + n * 30);
				btn.Size = new Size(gbNavigate.Width - 20, 25);
				btn.Text = name;
				btn.Tag = names[n];
				btn.Click += OnNavigation;
				gbNavigate.Controls.Add(btn);
				++n;
			}
		}

		protected void OnNavigation(object sender, EventArgs e)
		{
			string assocName = (string)((Button)sender).Tag;
			string fromSchemaName = assocName.LeftOf("_");
			string toSchemaName = assocName.RightOf("_");
			Schema fromSchema = model.GetSchema(fromSchemaName);
			Schema toSchema = model.GetSchema(toSchemaName);
			// Schema assocSchema = model.Db.GetAssociationSchema(fromSchema, toSchema);

			// TODO: Duplicate code, sort of (notice use of assocSchema and toSchema)
			BsonDocument filter = new BsonDocument(fromSchemaName + "Id", new ObjectId(dgvSemanticData.SelectedRow()["_id"].ToString()));
			// List<BsonDocument> docs = model.Db.Query(assocSchema, filter);
			List<BsonDocument> docs = model.Db.QueryAssociationServerSide(fromSchema, toSchema, filter);
			List<ConcreteType> semanticColumns;
			DataTable dt = DataHelpers.InitializeSemanticColumns(toSchema, out semanticColumns);
			DataHelpers.PopulateSemanticTable(dt, docs, toSchema);
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<AssociatedDataViewMembrane, ST_Data>(data => { data.Table = dt; data.Schema = toSchema; });
		}

		protected void OnSemanticTypeSelected(object sender, TreeViewEventArgs e)
		{
			object item = e.Node.Tag;

			if (item is Schema)
			{
				assocView.Update((Schema)item);
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
			TreeNode schemaRootNode = new TreeNode(GetSchemaNodeText(schema));
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
				TreeNode ctNode = cttn.Nodes.Add(GetConcreteTypeText(ct));
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

		protected string GetSchemaNodeText(Schema schema)
		{
			string ret = schema.Name;

			if (schema.IsAliased)
			{
				ret = ret + " (" + schema.Alias + ")";
			}

			return ret;
		}

		protected string GetConcreteTypeText(ConcreteType ct)
		{
			string ret = ct.Name;

			if (ct.IsAliased)
			{
				ret = ret + " (" + ct.Alias + ")";
			}

			return ret;
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
				string fwdDescr = tbFwdAssoc.Text;
				string revDescr = tbRevAssoc.Text;

				BsonDocument doc = new BsonDocument(fwdName + "Id", new ObjectId(fromId));
				doc.Add(revName + "Id", new ObjectId(toId));
				doc.Add("forwardAssociationName", tbFwdAssoc.Text);
				doc.Add("reverseAssociationName", tbRevAssoc.Text);
				model.Db.Insert(assocSchema, doc);
			}
		}
	}
}
