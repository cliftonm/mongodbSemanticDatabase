using System;
using System.Windows.Forms;

using Clifton.Core.Semantics;

namespace WinformExample
{
	public class LogView : IReceptor
	{
		protected TextBox tbLog;

		public LogView(TextBox tbLog)
		{
			this.tbLog = tbLog;
			Program.serviceManager.Get<ISemanticProcessor>().Register<LogViewMembrane>(this);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Log log)
		{
			string hms = DateTime.Now.ToString("HH:mm:ss");
			tbLog.FindForm().BeginInvoke(() => tbLog.AppendText("\r\n" + hms + " " + log.Message));
		}
	}
}
