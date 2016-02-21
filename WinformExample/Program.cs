using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformExample
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Model model = new Model();
			model.InitializeCoreSchemata();
			model.OpenDatabase("marc");
			Controller controller = new Controller(model);
			controller.InstantiateMissingLookups();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			SemanticDesigner view = new SemanticDesigner(model, controller);
			controller.View = view;
			Application.Run(view);
		}
	}
}
