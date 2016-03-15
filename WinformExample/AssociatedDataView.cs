using System;
using System.Data;
using System.Windows.Forms;

using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class AssociatedDataView : ServiceBase, IAssociatedDataViewService, IReceptor
	{
		public Schema Schema { get { return schema; } }
		public bool HasSelectedRow { get { return dgView.HasSelectedRow(); } }
		public DataRow SelectedRow { get { return dgView.SelectedRow(); } }

		public int SelectedRowIndex { get { return dgView.SelectedRowIndex(); } }
		public int NumRows { get { return dgView.NumRows(); } }

		protected Label label;
		protected DataGridView dgView;
		protected Schema schema;

		public AssociatedDataView(Label label, DataGridView view)
		{
			Program.serviceManager.RegisterSingleton<IAssociatedDataViewService>(this);
			Program.serviceManager.Get<ISemanticProcessor>().Register<AssociatedDataViewMembrane>(this);
			this.label = label;
			this.dgView = view;
		}

		public DataRow GetRowAt(int idx)
		{
			return ((DataView)dgView.DataSource)[idx].Row;
		}

		public void Clear()
		{
			dgView.DataSource = null;
			label.Text = "Semantic Type: ";
		}

		// TODO: Duplicate code
		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Data data)
		{
			schema = data.Schema;
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
