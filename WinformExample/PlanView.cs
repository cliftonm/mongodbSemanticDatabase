using System;
using System.Windows.Forms;

using Clifton.Core.Semantics;

namespace WinformExample
{
	public class PlanView : IReceptor
	{
		protected TextBox tbPlan;

		public PlanView(TextBox tbPlan)
		{
			this.tbPlan = tbPlan;
			Program.serviceManager.Get<ISemanticProcessor>().Register<PlanViewMembrane>(this);
		}

		public void Process(ISemanticProcessor proc, IMembrane membrane, ST_Plan plan)
		{
			tbPlan.FindForm().BeginInvoke(() => tbPlan.Text = plan.Plan);
		}
	}
}
