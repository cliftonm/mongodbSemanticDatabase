using System;
using System.Collections.Generic;

namespace Clifton.MongoSemanticDatabase
{
	public class ConcreteType
	{
		public string Name { get; set; }
		public string Alias { get; set; }
		public Type Type { get; set; }
	}

	public class Schema
	{
		public string Name { get; set; }
		public Type Type { get; set; }

		public List<Schema> Subtypes { get; set; }
		public List<ConcreteType> ConcreteTypes { get; set; }

		public bool IsConcreteType { get { return Subtypes.Count == 0; } }

		public Schema()
		{
			Subtypes = new List<Schema>();
			ConcreteTypes = new List<ConcreteType>();
		}
	}
}
