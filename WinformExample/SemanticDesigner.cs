using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public partial class SemanticDesigner : Form
	{
		protected Model model;

		protected CollectionView collectionView;
		protected SemanticView semanticView;
		protected AssociationView assocView;

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
			new AssociatedDataView(lblAssociatedData, dgvAssociationData);
			assocView = new AssociationView(model, dgvAssociations);
			semanticView = new SemanticView(lblSemanticType, dgvSemanticData);
			semanticController = new SemanticController(model);

			tvTypes.AfterSelect += semanticController.AfterSelectEvent;
			tvTypes.AfterSelect += OnSemanticTypeSelected;
			dgvSemanticData.SelectionChanged += semanticController.SelectionChangedEvent;

			collectionView = new CollectionView(lblCollectionName, dgvCollectionData);
			collectionController = new CollectionController(model);
			tvTypes.AfterSelect += collectionController.AfterSelectEvent;
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

		private void btnAssociateRecords_Click(object sender, EventArgs e)
		{
			
		}
	}
}
