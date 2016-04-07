using System;
using System.Data;
using System.Windows.Forms;

using Clifton.Core.Semantics;

namespace WinformExample
{
	public class CollectionView : IReceptor
	{
		protected Label label;
		protected DataGridView view;

		public CollectionView(Label label, DataGridView view)
		{
			this.label = label;
			this.view = view;
			Program.serviceManager.Get<ISemanticProcessor>().Register<CollectionViewMembrane>(this);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Data data)
		{
			DataView dv = new DataView(data.Table);
			view.FindForm().BeginInvoke(() =>
				{
					view.DataSource = dv;
					label.Text = "Collection: " + data.Table.TableName;
				});
		}

		protected void Process(ISemanticProcessor proc, IMembrane membrane, ST_NoData nothing)
		{
			view.FindForm().BeginInvoke(() =>
			{
				view.DataSource = null;
				label.Text = "Collection: ";
			});
		}
	}
}
