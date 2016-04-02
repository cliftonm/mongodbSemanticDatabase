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
						MenuItem addNewSubType = new MenuItem("Add New Subtype...");
						addNewSubType.Click += OnAddNewSubtype;
						cm.MenuItems.Add(addNewSubType);
						MenuItem addExistingSubType = new MenuItem("Add Existing Subtype...");
						addExistingSubType.Click += OnAddExistingSubtype;
						cm.MenuItems.Add(addExistingSubType);
						break;

					default:
						if (e.Node.Tag is Schema)
						{
							MenuItem removeSchema = new MenuItem("Remove Schema...");
							removeSchema.Click += OnRemoveSchemaClick;
							cm.MenuItems.Add(removeSchema);
							break;
						}
						else if (e.Node.Tag is ConcreteType)
						{
							MenuItem editConcreteType = new MenuItem("Edit Concrete Type...");
							editConcreteType.Click += OnEditConcreteType;
							editConcreteType.Tag = e.Node;
							cm.MenuItems.Add(editConcreteType);
							MenuItem removeConcreteType = new MenuItem("Remove Concrete Type...");
							removeConcreteType.Click += OnRemoveConcreteTypeClick;
							cm.MenuItems.Add(removeConcreteType);
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

		void OnEditConcreteType(object sender, EventArgs e)
		{
			new EditConcreteTypeDlg((TreeNode)((MenuItem)sender).Tag).ShowDialog();
		}

		protected void OnAddSchema(object sender, EventArgs e)
		{
			new AddSchemaDlg().ShowDialog();
		}

		protected void OnRemoveSchemaClick(object sender, EventArgs e)
		{
			Schema schema = (Schema)selectedNode.Tag;
			DialogResult res = MessageBox.Show("Remove collection in database as well?", schema.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);


			if (res == DialogResult.Yes)
			{
				model.Db.RemoveCollection(schema.Name);
			}

			selectedNode.Parent.Nodes.Remove(selectedNode);

			if (schema.Parent == null)
			{
				model.Schemata.Remove(schema);		
			}
			else
			{
				schema.Parent.Subtypes.Remove(schema);
			}
		}

		protected void OnRemoveConcreteTypeClick(object sender, EventArgs e)
		{
			ConcreteType ct = (ConcreteType)selectedNode.Tag;
			Schema schema = (Schema)selectedNode.Parent.Parent.Tag;			// skip the "Concrete Types" node.			
			selectedNode.Parent.Nodes.Remove(selectedNode);
			schema.ConcreteTypes.Remove(ct);
		}

		protected void OnAddConcreteType(object sender, EventArgs e)
		{
			new AddConcreteTypeDlg().ShowDialog();
		}

		protected void OnAddNewSubtype(object sender, EventArgs e)
		{
			new AddNewSubtypeDlg().ShowDialog();
		}

		/// <summary>
		/// Select from an existing subtype schema to replicate it at the currently selected node.
		/// </summary>
		protected void OnAddExistingSubtype(object sender, EventArgs e)
		{
			new AddExistingSubtypeDlg(model).ShowDialog();
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
			AddConcreteTypeToNode(cttn, ct);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AddNewSubtype data)
		{
			TreeNode sttn = selectedNode;
			Schema parentSchema = (Schema)sttn.Parent.Tag;
			Schema schema = new Schema() { Name = data.Name, Alias = data.Alias, Parent = parentSchema };
			parentSchema.Subtypes.Add(schema);
			AddSchemaNode(sttn, schema);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AddExistingSubtype data)
		{
			TreeNode sttn = selectedNode;
			Schema parentSchema = (Schema)sttn.Parent.Tag;
			Schema schema = data.Schema.DeepClone();
			schema.Parent = parentSchema;
			parentSchema.Subtypes.Add(schema);
			TreeNode tn = AddSchemaNode(sttn, schema);
			AddConcreteTypes(tn.Nodes[0], schema);
			schema.Subtypes.ForEach(st => AddSubTypesToNode(tn.Nodes[1], st));
		}

		protected TreeNode AddSchemaNode(TreeNode sttn, Schema schema)
		{
			TreeNode tn = new TreeNode(Helpers.GetSchemaNodeText(schema));
			tn.Tag = schema;
			tn.Nodes.Add("Concrete Types");
			tn.Nodes.Add("Subtypes");

			tv.FindForm().BeginInvoke(() =>
				{
					sttn.Nodes.Add(tn);
				});

			return tn;
		}

		protected void AddConcreteTypes(TreeNode tn, Schema schema)
		{
			schema.ConcreteTypes.ForEach(ct =>
				{
					AddConcreteTypeToNode(tn, ct);
				});
		}

		protected void AddSubTypesToNode(TreeNode tn, Schema schema)
		{
			TreeNode sttn = AddSchemaNode(tn, schema);
			AddConcreteTypes(sttn.Nodes[0], schema);
			schema.Subtypes.ForEach(st => AddSubTypesToNode(sttn.Nodes[1], st));
		}

		protected void AddConcreteTypeToNode(TreeNode cttn, ConcreteType ct)
		{
			tv.FindForm().BeginInvoke(() =>
			{
				TreeNode ctNode = cttn.Nodes.Add(Helpers.GetConcreteTypeText(ct));
				ctNode.Tag = ct;
			});
		}
	}
}
