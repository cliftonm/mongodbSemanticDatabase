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

		/// <summary>
		/// Parent will not be assigned (nor accurate) until FixupParents() is called.
		/// </summary>
		public Schema Parent { get; protected set; }

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

		public void FixupParents()
		{
			foreach (Schema subtype in Subtypes)
			{
				subtype.Parent = this;
				subtype.FixupParents();
			}
		}

		/// <summary>
		/// Recurse through the subtypes to get a list of all types.
		/// </summary>
		public List<Schema> GetTypes()
		{
			List<Schema> types = new List<Schema>();
			types.Add(this);

			foreach (Schema subtype in Subtypes)
			{
				types.AddRange(subtype.GetTypes());
			}

			return types;
		}

		/// <summary>
		/// Returns the chain of types, from bottom-most subtype to topmost supertype.
		/// </summary>
		public List<Schema> GetTypeChain()
		{
			List<Schema> parentChain = new List<Schema>();
			parentChain.Add(this);
			Schema schema = this;

			while (schema.Parent != null)
			{
				parentChain.Add(schema.Parent);
				schema = schema.Parent;
			}

			// parentChain.Reverse();

			return parentChain;
		}
	}
}
