using System;
using System.Windows.Forms;

using Clifton.Core.Semantics;

namespace WinformExample
{
	public partial class AddSubtypeDlg : Form
	{
		public AddSubtypeDlg()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<SemanticTreeMembrane, ST_AddSubtype>(s =>
			{
				s.Name = tbName.Text;
				s.Alias = tbAlias.Text;
			});

			Close();
		}
	}
}
