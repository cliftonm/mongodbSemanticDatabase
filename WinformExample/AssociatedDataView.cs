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

		protected Model model;
		protected Label label;
		protected DataGridView dgView;
		protected Schema schema;

		public AssociatedDataView(Model model, Label label, DataGridView view)
		{
			this.model = model;
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

		// TODO: Mostly duplicate code
		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_AssociatedData data)
		{
			model.ToData = data.Table;
			schema = data.Schema;
			DataView dv = new DataView(data.Table);
			dgView.FindForm().BeginInvoke(() =>
			{
				dgView.DataSource = dv;
				dgView.Columns[0].Visible = false;								// Hide the ID field.
				dgView.Columns[dgView.Columns.Count - 1].Visible = false;		// Also hide the last two ID fields, which are the ID's of the master and detail records.
				dgView.Columns[dgView.Columns.Count - 2].Visible = false;		// Also hide the last two ID fields, which are the ID's of the master and detail records.
				label.Text = "Semantic Type: " + data.Table.TableName;
			});
		}

		protected void Process(ISemanticProcessor proc, IMembrane membrane, ST_NoData nothing)
		{
			dgView.FindForm().BeginInvoke(() =>
			{
				dgView.DataSource = null;
				label.Text = "Collection: ";
			});
		}
	}
}
