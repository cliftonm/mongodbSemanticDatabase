using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.Semantics;
using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public partial class AddExistingSubtypeDlg : Form
	{
		public AddExistingSubtypeDlg(Model model)
		{
			InitializeComponent();
			PopulateSchemas(model);
		}

		protected void PopulateSchemas(Model model)
		{
			List<Schema> schemas = new List<Schema>();
			model.Schemata.ForEach(s=>AddSchema(s, schemas));
			// Sort and add to listbox.
			object[] schemaObjects = schemas.OrderBy(s => s.Name).ToArray();
			lbSchemas.Items.AddRange(schemaObjects);
		}

		/// <summary>
		/// Recurse into subtypes, adding only uniquely named schemas.
		/// </summary>
		protected void AddSchema(Schema schema, List<Schema> schemas)
		{
			schemas.AddIfUnique(schema, x => x.Name == schema.Name);
			schema.Subtypes.ForEach(s => AddSchema(s, schemas));
		}

		private void btnAddSchema_Click(object sender, EventArgs e)
		{
			Program.serviceManager.Get<ISemanticProcessor>().ProcessInstance<SemanticTreeMembrane, ST_AddExistingSubtype>(s =>
			{
				s.Schema = (Schema)lbSchemas.SelectedItem;
			});

			Close();
		}
	}
}
