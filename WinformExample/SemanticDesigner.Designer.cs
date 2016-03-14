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
			this.btnCreate = new System.Windows.Forms.Button();
			this.dgvAssociations = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			this.tbPlan = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dgvSemanticData = new System.Windows.Forms.DataGridView();
			this.lblSemanticType = new System.Windows.Forms.Label();
			this.dgvCollectionData = new System.Windows.Forms.DataGridView();
			this.lblCollectionName = new System.Windows.Forms.Label();
			this.lblAssociatedData = new System.Windows.Forms.Label();
			this.dgvAssociationData = new System.Windows.Forms.DataGridView();
			this.btnAssociateRecords = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociations)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociationData)).BeginInit();
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
			this.splitContainer1.Panel2.Controls.Add(this.btnAssociateRecords);
			this.splitContainer1.Panel2.Controls.Add(this.dgvAssociationData);
			this.splitContainer1.Panel2.Controls.Add(this.lblAssociatedData);
			this.splitContainer1.Panel2.Controls.Add(this.btnCreate);
			this.splitContainer1.Panel2.Controls.Add(this.dgvAssociations);
			this.splitContainer1.Panel2.Controls.Add(this.label3);
			this.splitContainer1.Panel2.Controls.Add(this.tbPlan);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.tbLog);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.dgvSemanticData);
			this.splitContainer1.Panel2.Controls.Add(this.lblSemanticType);
			this.splitContainer1.Panel2.Controls.Add(this.dgvCollectionData);
			this.splitContainer1.Panel2.Controls.Add(this.lblCollectionName);
			this.splitContainer1.Size = new System.Drawing.Size(1355, 732);
			this.splitContainer1.SplitterDistance = 282;
			this.splitContainer1.TabIndex = 0;
			// 
			// tvTypes
			// 
			this.tvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvTypes.FullRowSelect = true;
			this.tvTypes.HideSelection = false;
			this.tvTypes.Location = new System.Drawing.Point(0, 0);
			this.tvTypes.Name = "tvTypes";
			this.tvTypes.Size = new System.Drawing.Size(282, 732);
			this.tvTypes.TabIndex = 0;
			// 
			// btnCreate
			// 
			this.btnCreate.Location = new System.Drawing.Point(21, 477);
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Size = new System.Drawing.Size(75, 23);
			this.btnCreate.TabIndex = 10;
			this.btnCreate.Text = "Create...";
			this.btnCreate.UseVisualStyleBackColor = true;
			this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
			// 
			// dgvAssociations
			// 
			this.dgvAssociations.AllowUserToAddRows = false;
			this.dgvAssociations.AllowUserToDeleteRows = false;
			this.dgvAssociations.AllowUserToResizeRows = false;
			this.dgvAssociations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAssociations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssociations.Location = new System.Drawing.Point(21, 264);
			this.dgvAssociations.MultiSelect = false;
			this.dgvAssociations.Name = "dgvAssociations";
			this.dgvAssociations.ReadOnly = true;
			this.dgvAssociations.RowHeadersVisible = false;
			this.dgvAssociations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAssociations.Size = new System.Drawing.Size(343, 206);
			this.dgvAssociations.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 245);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "Associations:";
			// 
			// tbPlan
			// 
			this.tbPlan.Location = new System.Drawing.Point(446, 555);
			this.tbPlan.Multiline = true;
			this.tbPlan.Name = "tbPlan";
			this.tbPlan.ReadOnly = true;
			this.tbPlan.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbPlan.Size = new System.Drawing.Size(405, 165);
			this.tbPlan.TabIndex = 7;
			this.tbPlan.WordWrap = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(443, 536);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "Plan:";
			// 
			// tbLog
			// 
			this.tbLog.Location = new System.Drawing.Point(18, 555);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbLog.Size = new System.Drawing.Size(405, 165);
			this.tbLog.TabIndex = 5;
			this.tbLog.WordWrap = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 537);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "Log:";
			// 
			// dgvSemanticData
			// 
			this.dgvSemanticData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvSemanticData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSemanticData.Location = new System.Drawing.Point(388, 32);
			this.dgvSemanticData.Name = "dgvSemanticData";
			this.dgvSemanticData.RowHeadersVisible = false;
			this.dgvSemanticData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvSemanticData.Size = new System.Drawing.Size(490, 193);
			this.dgvSemanticData.TabIndex = 3;
			// 
			// lblSemanticType
			// 
			this.lblSemanticType.AutoSize = true;
			this.lblSemanticType.Location = new System.Drawing.Point(385, 13);
			this.lblSemanticType.Name = "lblSemanticType";
			this.lblSemanticType.Size = new System.Drawing.Size(91, 15);
			this.lblSemanticType.TabIndex = 2;
			this.lblSemanticType.Text = "Semantic Type:";
			// 
			// dgvCollectionData
			// 
			this.dgvCollectionData.AllowUserToAddRows = false;
			this.dgvCollectionData.AllowUserToDeleteRows = false;
			this.dgvCollectionData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvCollectionData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCollectionData.Location = new System.Drawing.Point(18, 32);
			this.dgvCollectionData.Name = "dgvCollectionData";
			this.dgvCollectionData.ReadOnly = true;
			this.dgvCollectionData.RowHeadersVisible = false;
			this.dgvCollectionData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvCollectionData.Size = new System.Drawing.Size(346, 193);
			this.dgvCollectionData.TabIndex = 1;
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
			// lblAssociatedData
			// 
			this.lblAssociatedData.AutoSize = true;
			this.lblAssociatedData.Location = new System.Drawing.Point(388, 244);
			this.lblAssociatedData.Name = "lblAssociatedData";
			this.lblAssociatedData.Size = new System.Drawing.Size(101, 15);
			this.lblAssociatedData.TabIndex = 12;
			this.lblAssociatedData.Text = "Association Data:";
			// 
			// dgvAssociationData
			// 
			this.dgvAssociationData.AllowUserToAddRows = false;
			this.dgvAssociationData.AllowUserToDeleteRows = false;
			this.dgvAssociationData.AllowUserToResizeRows = false;
			this.dgvAssociationData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAssociationData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssociationData.Location = new System.Drawing.Point(391, 264);
			this.dgvAssociationData.MultiSelect = false;
			this.dgvAssociationData.Name = "dgvAssociationData";
			this.dgvAssociationData.ReadOnly = true;
			this.dgvAssociationData.RowHeadersVisible = false;
			this.dgvAssociationData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAssociationData.Size = new System.Drawing.Size(487, 206);
			this.dgvAssociationData.TabIndex = 13;
			// 
			// btnAssociateRecords
			// 
			this.btnAssociateRecords.Location = new System.Drawing.Point(593, 231);
			this.btnAssociateRecords.Name = "btnAssociateRecords";
			this.btnAssociateRecords.Size = new System.Drawing.Size(75, 23);
			this.btnAssociateRecords.TabIndex = 14;
			this.btnAssociateRecords.Text = "Associate";
			this.btnAssociateRecords.UseVisualStyleBackColor = true;
			this.btnAssociateRecords.Click += new System.EventHandler(this.btnAssociateRecords_Click);
			// 
			// SemanticDesigner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1355, 732);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SemanticDesigner";
			this.Text = "Semantic Designer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociations)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociationData)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView tvTypes;
		private System.Windows.Forms.DataGridView dgvCollectionData;
		private System.Windows.Forms.Label lblCollectionName;
		private System.Windows.Forms.DataGridView dgvSemanticData;
		private System.Windows.Forms.Label lblSemanticType;
		private System.Windows.Forms.TextBox tbLog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPlan;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.DataGridView dgvAssociations;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnAssociateRecords;
		private System.Windows.Forms.DataGridView dgvAssociationData;
		private System.Windows.Forms.Label lblAssociatedData;
	}
}

