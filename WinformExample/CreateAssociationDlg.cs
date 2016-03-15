using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;

using Clifton.MongoSemanticDatabase;

namespace WinformExample
{
	public partial class CreateAssociationDlg : Form
	{
		protected Model model;
		protected Schema fromSchema;

		public CreateAssociationDlg(Model model, Schema fromSchema)
		{
			this.model = model;
			this.fromSchema = fromSchema;
			InitializeComponent();
			PopulateSemanticTypes();
		}

		protected void btnCreate_Click(object sender, EventArgs e)
		{
			if (dgvTypes.HasSelectedRow())
			{
				string name = dgvTypes.SelectedRow()[0].ToString();
				Schema toSchema = model.GetSchema(name);

				if (toSchema == null)
				{
					DialogResult ret = MessageBox.Show("The schema for this semantic type must be discovered.", "Proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

					if (ret == DialogResult.Yes)
					{
						toSchema = model.Db.DiscoverSchema(name);
					}
				}

				if (toSchema != null)
				{
					// TODO: Check if the association already exists, either forwards or reverse
					try
					{
						model.Db.Associate(fromSchema, toSchema);
						Close();
					}
					catch (SemanticDatabaseException ex)
					{
						MessageBox.Show(ex.Message, "Not Possible", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					}
				}
			}
		}

		protected void PopulateSemanticTypes()
		{
			List<string> collections = model.Db.GetCollections();

			// Add in our schemata, in case we define a schema for which there is no data yet, we won't see it in the list of collections, for some reason!
			model.Schemata.ForEach(s =>
			{
				if (!collections.Contains(s.Name))
				{
					collections.Add(s.Name);
				}
			});

			collections = collections.OrderBy(k => k).ToList();

			DataTable dt = Helpers.FillTable(collections, "Schema");
			DataView dv = new DataView(dt);
			dgvTypes.DataSource = dv;
		}
	}
}
