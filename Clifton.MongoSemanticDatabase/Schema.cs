using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Clifton.MongoSemanticDatabase
{
	public class ConcreteType
	{
		protected string alias;
	
		public string Name { get; set; }

		public string Alias
		{
			get { return alias ?? Name; }
			// Force to null if empty string.
			set { alias = String.IsNullOrEmpty(value) ? null : value; }
		}

		public bool IsAliased { get { return Alias != Name; } }

		public Type Type { get; set; }

		public ConcreteType Clone()
		{
			ConcreteType ct = new ConcreteType()
			{
				Name = this.Name,
				Alias = this.Alias,
				Type = this.Type
			};

			return ct;
		}
	}

	public class Schema
	{
		protected string alias;

		public string Name { get; set; }
		public string NameAsId { get { return Name + "Id"; } }
		
		public string Alias
		{
			get { return alias ?? Name; }
			set { alias = String.IsNullOrEmpty(value) ? null : value; }
		}

		public bool IsAliased { get { return Alias != Name; } }

		public List<Schema> Subtypes { get; set; }
		public List<ConcreteType> ConcreteTypes { get; set; }

		/// <summary>
		/// Parent will not be assigned (nor accurate) until FixupParents() is called.
		/// </summary>
		[JsonIgnore]
		public Schema Parent { get; set; }

		public bool IsConcreteType { get { return Subtypes.Count == 0; } }

		public Schema()
		{
			Subtypes = new List<Schema>();
			ConcreteTypes = new List<ConcreteType>();
		}

		public override string ToString()
		{
			return Name;
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

		/// <summary>
		/// Does not copy alias.
		/// </summary>
		public Schema DeepClone(Schema parent = null)
		{
			Schema schema = new Schema()
			{
				Name = this.Name,
				Alias = this.Alias,
				Parent = parent
			};

			foreach (ConcreteType ct in ConcreteTypes)
			{
				schema.ConcreteTypes.Add(ct.Clone());
			}

			foreach (Schema st in Subtypes)
			{
				schema.Subtypes.Add(st.DeepClone(schema));
			}

			return schema;
		}
	}
}
