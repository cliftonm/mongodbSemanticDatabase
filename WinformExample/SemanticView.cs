using System;
using System.Data;
using System.Windows.Forms;

using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;

namespace WinformExample
{
	public class SemanticView : ServiceBase, ISemanticViewService, IReceptor
	{
		public bool HasSelectedRow { get { return dgView.SelectedRows.Count != 0; } }
		public DataRow SelectedRow { get { return ((DataView)dgView.DataSource)[dgView.SelectedRows[0].Index].Row; } }

		public int SelectedRowIndex { get { return HasSelectedRow ? dgView.SelectedRows[0].Index : -2; } }
		public int NumRows { get { return ((DataView)dgView.DataSource).Table.Rows.Count; } }

		protected Label label;
		protected DataGridView dgView;

		public SemanticView(Label label, DataGridView view)
		{
			Program.serviceManager.RegisterSingleton<ISemanticViewService>(this);
			Program.serviceManager.Get<ISemanticProcessor>().Register<SemanticViewMembrane>(this);
			this.label = label;
			this.dgView = view;
		}

		public DataRow GetRowAt(int idx)
		{
			return ((DataView)dgView.DataSource)[idx].Row;
		}

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
