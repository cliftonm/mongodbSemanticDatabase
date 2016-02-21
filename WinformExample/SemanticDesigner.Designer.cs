namespace WinformExample
{
	partial class SemanticDesigner
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tvTypes = new System.Windows.Forms.TreeView();
			this.lblCollectionName = new System.Windows.Forms.Label();
			this.dgvCollectionData = new System.Windows.Forms.DataGridView();
			this.lblSemanticType = new System.Windows.Forms.Label();
			this.dgvSemanticData = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tvTypes);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dgvSemanticData);
			this.splitContainer1.Panel2.Controls.Add(this.lblSemanticType);
			this.splitContainer1.Panel2.Controls.Add(this.dgvCollectionData);
			this.splitContainer1.Panel2.Controls.Add(this.lblCollectionName);
			this.splitContainer1.Size = new System.Drawing.Size(1130, 732);
			this.splitContainer1.SplitterDistance = 236;
			this.splitContainer1.TabIndex = 0;
			// 
			// tvTypes
			// 
			this.tvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvTypes.FullRowSelect = true;
			this.tvTypes.HideSelection = false;
			this.tvTypes.Location = new System.Drawing.Point(0, 0);
			this.tvTypes.Name = "tvTypes";
			this.tvTypes.Size = new System.Drawing.Size(236, 732);
			this.tvTypes.TabIndex = 0;
			// 
			// lblCollectionName
			// 
			this.lblCollectionName.AutoSize = true;
			this.lblCollectionName.Location = new System.Drawing.Point(15, 13);
			this.lblCollectionName.Name = "lblCollectionName";
			this.lblCollectionName.Size = new System.Drawing.Size(64, 15);
			this.lblCollectionName.TabIndex = 0;
			this.lblCollectionName.Text = "Collection:";
			// 
			// dgvCollectionData
			// 
			this.dgvCollectionData.AllowUserToAddRows = false;
			this.dgvCollectionData.AllowUserToDeleteRows = false;
			this.dgvCollectionData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvCollectionData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCollectionData.Location = new System.Drawing.Point(18, 32);
			this.dgvCollectionData.Name = "dgvCollectionData";
			this.dgvCollectionData.ReadOnly = true;
			this.dgvCollectionData.RowHeadersVisible = false;
			this.dgvCollectionData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvCollectionData.Size = new System.Drawing.Size(860, 193);
			this.dgvCollectionData.TabIndex = 1;
			// 
			// lblSemanticType
			// 
			this.lblSemanticType.AutoSize = true;
			this.lblSemanticType.Location = new System.Drawing.Point(18, 242);
			this.lblSemanticType.Name = "lblSemanticType";
			this.lblSemanticType.Size = new System.Drawing.Size(91, 15);
			this.lblSemanticType.TabIndex = 2;
			this.lblSemanticType.Text = "Semantic Type:";
			// 
			// dgvSemanticData
			// 
			this.dgvSemanticData.AllowUserToAddRows = false;
			this.dgvSemanticData.AllowUserToDeleteRows = false;
			this.dgvSemanticData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvSemanticData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSemanticData.Location = new System.Drawing.Point(18, 260);
			this.dgvSemanticData.Name = "dgvSemanticData";
			this.dgvSemanticData.ReadOnly = true;
			this.dgvSemanticData.RowHeadersVisible = false;
			this.dgvSemanticData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvSemanticData.Size = new System.Drawing.Size(860, 193);
			this.dgvSemanticData.TabIndex = 3;
			// 
			// SemanticDesigner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1130, 732);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SemanticDesigner";
			this.Text = "Semantic Designer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView tvTypes;
		private System.Windows.Forms.DataGridView dgvCollectionData;
		private System.Windows.Forms.Label lblCollectionName;
		private System.Windows.Forms.DataGridView dgvSemanticData;
		private System.Windows.Forms.Label lblSemanticType;
	}
}

