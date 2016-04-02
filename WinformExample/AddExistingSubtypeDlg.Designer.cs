namespace WinformExample
{
	partial class AddExistingSubtypeDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lbSchemas = new System.Windows.Forms.ListBox();
			this.btnAddSchema = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select Schema:";
			// 
			// lbSchemas
			// 
			this.lbSchemas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lbSchemas.FormattingEnabled = true;
			this.lbSchemas.Location = new System.Drawing.Point(13, 32);
			this.lbSchemas.Name = "lbSchemas";
			this.lbSchemas.Size = new System.Drawing.Size(286, 433);
			this.lbSchemas.TabIndex = 1;
			// 
			// btnAddSchema
			// 
			this.btnAddSchema.Location = new System.Drawing.Point(316, 32);
			this.btnAddSchema.Name = "btnAddSchema";
			this.btnAddSchema.Size = new System.Drawing.Size(114, 23);
			this.btnAddSchema.TabIndex = 2;
			this.btnAddSchema.Text = "Add Schema";
			this.btnAddSchema.UseVisualStyleBackColor = true;
			this.btnAddSchema.Click += new System.EventHandler(this.btnAddSchema_Click);
			// 
			// AddExistingSubtypeDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(445, 472);
			this.Controls.Add(this.btnAddSchema);
			this.Controls.Add(this.lbSchemas);
			this.Controls.Add(this.label1);
			this.Name = "AddExistingSubtypeDlg";
			this.ShowInTaskbar = false;
			this.Text = "Add Existing Subtype";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lbSchemas;
		private System.Windows.Forms.Button btnAddSchema;
	}
}