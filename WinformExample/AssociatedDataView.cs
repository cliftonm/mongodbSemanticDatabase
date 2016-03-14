using System;
using System.Data;
using System.Windows.Forms;

using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;

namespace WinformExample
{
	public class AssociatedDataView : ServiceBase, IAssociatedDataViewService, IReceptor
	{
		protected Label label;
		protected DataGridView dgView;

		public AssociatedDataView(Label label, DataGridView view)
		{
			Program.serviceManager.RegisterSingleton<IAssociatedDataViewService>(this);
			Program.serviceManager.Get<ISemanticProcessor>().Register<AssociatedDataViewMembrane>(this);
			this.label = label;
			this.dgView = view;
		}

		// TODO: Duplicate code
		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Data data)
		{
			DataView dv = new DataView(data.Table);
			dgView.FindForm().BeginInvoke(() =>
				{
					dgView.DataSource = dv;
					dgView.Columns[0].Visible = false;			// Hide the ID field.
					label.Text = "Semantic Type: " + data.Table.TableName;
				});
		}
	}
}
