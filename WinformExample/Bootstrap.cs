using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.Semantics;
using Clifton.Core.ModuleManagement;
using Clifton.Core.ServiceManagement;

namespace WinformExample
{
	static partial class Program
	{
		public static ServiceManager serviceManager;

		public static void Bootstrap()
		{
			serviceManager = new ServiceManager();
			serviceManager.RegisterSingleton<IModuleManager, ModuleManager>();

			try
			{
				IModuleManager moduleMgr = serviceManager.Get<IModuleManager>();
				List<AssemblyFileName> moduleFilenames = moduleMgr.GetModuleList(XmlFileName.Create("modules.xml"));

				moduleMgr.RegisterModules(moduleFilenames);
				serviceManager.FinishedInitialization();
			}
			catch (ReflectionTypeLoadException ex)
			{
				foreach (var item in ex.LoaderExceptions)
				{
					MessageBox.Show(item.Message + "\r\n" + ex.StackTrace, "Reflection Type Loader Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(1);
			}
		}
	}
}
