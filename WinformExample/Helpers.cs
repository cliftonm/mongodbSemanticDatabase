using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;

namespace WinformExample
{
	public static class Helpers
	{
		public static void Log(string msg)
		{
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<LogViewMembrane, ST_Log>(log => log.Message = msg);
		}

		public static DataTable FillTable(List<string> list, string colName)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(colName);

			foreach (string item in list)
			{
				DataRow row = dt.NewRow();
				row[0] = item;
				dt.Rows.Add(row);
			}

			return dt;
		}

		public static void Try(Action activity)
		{
			try
			{
				activity();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// Extension Helpers:

		/// <summary>
		/// Asynchronous invoke on application thread.  Will return immediately unless invocation is not required.
		/// </summary>
		public static void BeginInvoke(this Control control, Action action)
		{
			if (control.InvokeRequired)
			{
				// We want a synchronous call here!!!!
				control.BeginInvoke((Delegate)action);
			}
			else
			{
				action();
			}
		}

		/// <summary>
		/// Synchronous invoke on application thread.  Will not return until action is completed.
		/// </summary>
		public static void Invoke(this Control control, Action action)
		{
			if (control.InvokeRequired)
			{
				// We want a synchronous call here!!!!
				control.Invoke((Delegate)action);
			}
			else
			{
				action();
			}
		}

		public static bool HasSelectedRow(this DataGridView view)
		{
			return view.SelectedRows.Count != 0;
		}

		public static DataRow SelectedRow(this DataGridView view)
		{
			return ((DataView)view.DataSource)[view.SelectedRows[0].Index].Row;
		}

		public static int SelectedRowIndex(this DataGridView view)
		{
			return view.HasSelectedRow() ? view.SelectedRows[0].Index : -2;
		}

		public static int NumRows(this DataGridView view)
		{
			return ((DataView)view.DataSource).Table.Rows.Count;
		}
	}
}
