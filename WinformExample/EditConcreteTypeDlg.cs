using System;
using System.Windows.Forms;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public partial class EditConcreteTypeDlg : Form
	{
		protected ConcreteType concreteType;
		protected TreeNode node;

		public EditConcreteTypeDlg(TreeNode node)
		{
			this.node = node;
			concreteType = (ConcreteType)node.Tag;
			InitializeComponent();
			tbName.Text = concreteType.Name;
			tbAlias.Text = concreteType.IsAliased ? concreteType.Alias : null;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			concreteType.Name = tbName.Text;
			concreteType.Alias = tbAlias.Text;
			node.Text = Helpers.GetConcreteTypeText(concreteType);
			Close();
		}
	}
}
