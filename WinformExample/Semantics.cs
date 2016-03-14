using System;
using System.Data;

using Clifton.Core.Semantics;

namespace WinformExample
{
	public class CollectionViewMembrane : Membrane { }
	public class SemanticViewMembrane : Membrane { }
	public class LogViewMembrane : Membrane { }
	public class PlanViewMembrane : Membrane { }
	public class AssociatedDataViewMembrane : Membrane { }

	public class ST_Log : ISemanticType
	{
		public string Message { get; set; }
	}

	public class ST_Plan : ISemanticType
	{
		public string Plan { get; set; }
	}

	public class ST_Data : ISemanticType
	{
		public DataTable Table { get; set; }
	}
}
