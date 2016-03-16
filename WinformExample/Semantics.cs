using System;
using System.Collections.Generic;
using System.Data;

using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public class SemanticTreeMembrane : Membrane { }
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

	public class ST_AddSchema : ISemanticType
	{
		public string Name { get; set; }
		public string Alias { get; set; }
	}

	public class ST_AddConcreteType : ISemanticType
	{
		public string Name { get; set; }
		public string Alias { get; set; }
		// public string Type { get; set; }		// TODO: Implement type at some point.
	}

	/// <summary>
	/// A subtype is actually a schema.
	/// </summary>
	public class ST_AddSubtype : ISemanticType
	{
		public string Name { get; set; }
		public string Alias { get; set; }
	}
}
