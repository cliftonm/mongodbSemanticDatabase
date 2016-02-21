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
		protected Controller controller;

		public SemanticDesigner(Model model, Controller controller)
		{
			this.model = model;
			InitializeComponent();
			InitializeSchemaView();
			tvTypes.ExpandAll();

			tvTypes.AfterSelect += controller.AfterSelectEvent;
		}

		public void ShowCollectionData(DataTable dt)
		{
			DataView dv = new DataView(dt);
			dgvCollectionData.DataSource = dv;
			lblCollectionName.Text = "Collection: " + dt.TableName;
		}

		public void ShowSemanticData(DataTable dt)
		{
			DataView dv = new DataView(dt);
			dgvSemanticData.DataSource = dv;
			lblSemanticType.Text = "Semantic Type: " + dt.TableName;
		}

		protected void InitializeSchemaView()
		{
			TreeNode root = tvTypes.Nodes.Add("Semantic Schema");

			foreach (Schema schema in model.Schemata)
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
	}
}
