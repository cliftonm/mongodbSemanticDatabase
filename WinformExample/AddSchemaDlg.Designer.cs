namespace WinformExample
{
	partial class AddSchemaDlg
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
			this.tbName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbAlias = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// tbName
			// 
			this.tbName.Location = new System.Drawing.Point(63, 10);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(194, 20);
			this.tbName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Alias:";
			// 
			// tbAlias
			// 
			this.tbAlias.Location = new System.Drawing.Point(63, 36);
			this.tbAlias.Name = "tbAlias";
			this.tbAlias.Size = new System.Drawing.Size(194, 20);
			this.tbAlias.TabIndex = 3;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(277, 8);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// AddSchemaDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(368, 75);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.tbAlias);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbName);
			this.Controls.Add(this.label1);
			this.Name = "AddSchemaDlg";
			this.Text = "Add New Schema";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbAlias;
		private System.Windows.Forms.Button btnAdd;
	}
}