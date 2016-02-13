using System;
using System.Collections.Generic;
using System.Linq;

namespace Clifton.MongoSemanticDatabase
{
	public class ConcreteType
	{
		protected string alias;
	
		public string Name { get; set; }

		public string Alias
		{
			get { return alias ?? Name; }
			set { alias = value; }
		}

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

		public bool ContainsAliasedType(string alias, out string name)
		{
			name = null;
			bool ret = false;

			foreach (ConcreteType ct in ConcreteTypes)
			{
				if (ct.Alias == alias)
				{
					name = ct.Name;
					ret = true;
					break;
				}
			}

			return ret;
		}

		public string GetAlias(string name)
		{
			return ConcreteTypes.Single(ct => ct.Name == name).Alias;
		}
	}
}
