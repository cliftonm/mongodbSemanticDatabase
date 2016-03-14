using System;
using System.Windows.Forms;

using Clifton.Core.Semantics;

namespace WinformExample
{
	static partial class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Bootstrap();
			Model model = new Model();
			model.InitializeCoreSchemata();
			model.OpenDatabase("marc");

			// General controller.
			Controller controller = new Controller(model);
			controller.InstantiateMissingLookups();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			SemanticDesigner view = new SemanticDesigner(model);

			Application.Run(view);
		}
	}
}
