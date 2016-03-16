using System;
using System.Linq;
using System.Windows.Forms;

using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class SemanticTreeView : IReceptor
	{
		protected Model model;
		protected TreeView tv;
		protected TreeNode selectedNode;

		public SemanticTreeView(Model model, TreeView tv)
		{
			this.model = model;
			this.tv = tv;
			Program.serviceManager.Get<ISemanticProcessor>().Register<SemanticTreeMembrane>(this);
			tv.NodeMouseClick += OnNodeMouseClick;
		}

		protected void OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			ContextMenu cm = new ContextMenu();

			if (e.Button == MouseButtons.Right)
			{
				bool somethingToDo = true;

				switch (e.Node.Text)
				{
					case "Semantic Schema":
						MenuItem addSchema = new MenuItem("Add Schema...");
						addSchema.Click += OnAddSchema;
						cm.MenuItems.Add(addSchema);
						break;

					case "Concrete Types":
						MenuItem addConcreteType = new MenuItem("Add Concrete Type...");
						addConcreteType.Click += OnAddConcreteType;
						cm.MenuItems.Add(addConcreteType);
						break;

					case "Subtypes":
						MenuItem addSubType = new MenuItem("Add Subtype...");
						addSubType.Click += OnAddSubtype;
						cm.MenuItems.Add(addSubType);
						break;

					default:
						if (e.Node.Tag is Schema)
						{
							MenuItem removeSchema = new MenuItem("Remove Schema...");
							removeSchema.Click += OnRemoveClick;
							cm.MenuItems.Add(removeSchema);
							break;
						}
						else
						{
							somethingToDo = false;
						}
						break;
				}

				if (somethingToDo)
				{
					tv.SelectedNode = e.Node;
					selectedNode = e.Node;
					cm.Show(tv, e.Location);
				}
			}
		}

		protected void OnAddSchema(object sender, EventArgs e)
		{
			new AddSchemaDlg().ShowDialog();
		}

		void OnRemoveClick(object sender, EventArgs e)
		{
			Schema schema = (Schema)selectedNode.Tag;
			DialogResult res = MessageBox.Show("Remove collection in database as well?", schema.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);


			if (res == DialogResult.Yes)
			{
				model.Db.RemoveCollection(schema.Name);
			}

			selectedNode.Parent.Nodes.Remove(selectedNode);
			model.Schemata.Remove(model.GetSchema(schema.Name));		// we find the instance in the collection to remove it.
		}

		protected void OnAddConcreteType(object sender, EventArgs e)
		{
			new AddConcreteTypeDlg().ShowDialog();
		}

		protected void OnAddSubtype(object sender, EventArgs e)
		{
			new AddSubtypeDlg().ShowDialog();
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AddSchema data)
		{
			Schema schema = new Schema() { Name = data.Name, Alias = data.Alias };
			model.Schemata.Add(schema);
			TreeNode tn = new TreeNode(Helpers.GetSchemaNodeText(schema));
			tn.Tag = schema;
			tn.Nodes.Add("Concrete Types");
			tn.Nodes.Add("Subtypes");

			tv.FindForm().BeginInvoke(() =>
				{
					tv.Nodes[0].Nodes.Add(tn);
				});
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AddConcreteType data)
		{
			TreeNode cttn = selectedNode;
			Schema schema = (Schema)cttn.Parent.Tag;
			// TODO: Get actual type at some point.
			ConcreteType ct = new ConcreteType() { Name = data.Name, Alias = data.Alias, Type = typeof(string) };
			schema.ConcreteTypes.Add(ct);

			tv.FindForm().BeginInvoke(() =>
				{
					TreeNode ctNode = cttn.Nodes.Add(Helpers.GetConcreteTypeText(ct));
					ctNode.Tag = ct;
				});
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AddSubtype data)
		{
			TreeNode sttn = selectedNode;
			Schema parentSchema = (Schema)sttn.Parent.Tag;
			Schema schema = new Schema() { Name = data.Name, Alias = data.Alias };
			parentSchema.Subtypes.Add(schema);

			TreeNode tn = new TreeNode(Helpers.GetSchemaNodeText(schema));
			tn.Tag = schema;
			tn.Nodes.Add("Concrete Types");
			tn.Nodes.Add("Subtypes");

			tv.FindForm().BeginInvoke(() =>
				{
					sttn.Nodes.Add(tn);
				});
		}
	}
}
