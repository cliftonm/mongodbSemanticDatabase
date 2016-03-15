using System;
using System.Collections.Generic;
using System.Data;

using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class CollectionViewMembrane : Membrane { }
	public class SemanticViewMembrane : Membrane { }
	public class LogViewMembrane : Membrane { }
	public class PlanViewMembrane : Membrane { }
	public class AssociationViewMembrane : Membrane { }
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
		public Schema Schema { get; set; }
	}

	public class ST_Associations : ISemanticType
	{
		public Schema AssocSchema { get; set; }
		public List<string> ForwardSchemaNames { get; set; }
		public List<string> ReverseSchemaNames { get; set; }
	}
}
