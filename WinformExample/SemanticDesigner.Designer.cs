﻿namespace WinformExample
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
			this.btnDeleteAssoc = new System.Windows.Forms.Button();
			this.gbNavigate = new System.Windows.Forms.GroupBox();
			this.tbRevAssoc = new System.Windows.Forms.TextBox();
			this.tbFwdAssoc = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.lblFwdAssoc = new System.Windows.Forms.Label();
			this.btnAssociateRecords = new System.Windows.Forms.Button();
			this.dgvAssociationData = new System.Windows.Forms.DataGridView();
			this.lblAssociatedData = new System.Windows.Forms.Label();
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuLoadSchema = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuImportSchema = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSaveSchema = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.btnMainView = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociationData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociations)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(0, 33);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tvTypes);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.btnMainView);
			this.splitContainer1.Panel2.Controls.Add(this.btnDeleteAssoc);
			this.splitContainer1.Panel2.Controls.Add(this.gbNavigate);
			this.splitContainer1.Panel2.Controls.Add(this.tbRevAssoc);
			this.splitContainer1.Panel2.Controls.Add(this.tbFwdAssoc);
			this.splitContainer1.Panel2.Controls.Add(this.label5);
			this.splitContainer1.Panel2.Controls.Add(this.lblFwdAssoc);
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
			this.splitContainer1.Size = new System.Drawing.Size(1355, 699);
			this.splitContainer1.SplitterDistance = 231;
			this.splitContainer1.TabIndex = 0;
			// 
			// tvTypes
			// 
			this.tvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvTypes.FullRowSelect = true;
			this.tvTypes.HideSelection = false;
			this.tvTypes.Location = new System.Drawing.Point(0, 0);
			this.tvTypes.Name = "tvTypes";
			this.tvTypes.Size = new System.Drawing.Size(231, 699);
			this.tvTypes.TabIndex = 0;
			// 
			// btnDeleteAssoc
			// 
			this.btnDeleteAssoc.Location = new System.Drawing.Point(391, 501);
			this.btnDeleteAssoc.Name = "btnDeleteAssoc";
			this.btnDeleteAssoc.Size = new System.Drawing.Size(116, 23);
			this.btnDeleteAssoc.TabIndex = 20;
			this.btnDeleteAssoc.Text = "Delete Assoc.";
			this.btnDeleteAssoc.UseVisualStyleBackColor = true;
			this.btnDeleteAssoc.Click += new System.EventHandler(this.btnDeleteAssoc_Click);
			// 
			// gbNavigate
			// 
			this.gbNavigate.Location = new System.Drawing.Point(885, 13);
			this.gbNavigate.Name = "gbNavigate";
			this.gbNavigate.Size = new System.Drawing.Size(172, 481);
			this.gbNavigate.TabIndex = 19;
			this.gbNavigate.TabStop = false;
			this.gbNavigate.Text = "Navigate To:";
			// 
			// tbRevAssoc
			// 
			this.tbRevAssoc.Location = new System.Drawing.Point(625, 250);
			this.tbRevAssoc.Name = "tbRevAssoc";
			this.tbRevAssoc.Size = new System.Drawing.Size(163, 20);
			this.tbRevAssoc.TabIndex = 18;
			// 
			// tbFwdAssoc
			// 
			this.tbFwdAssoc.Location = new System.Drawing.Point(625, 230);
			this.tbFwdAssoc.Name = "tbFwdAssoc";
			this.tbFwdAssoc.Size = new System.Drawing.Size(163, 20);
			this.tbFwdAssoc.TabIndex = 17;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(551, 252);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(66, 15);
			this.label5.TabIndex = 16;
			this.label5.Text = "Rev Assoc:";
			// 
			// lblFwdAssoc
			// 
			this.lblFwdAssoc.AutoSize = true;
			this.lblFwdAssoc.Location = new System.Drawing.Point(551, 233);
			this.lblFwdAssoc.Name = "lblFwdAssoc";
			this.lblFwdAssoc.Size = new System.Drawing.Size(68, 15);
			this.lblFwdAssoc.TabIndex = 15;
			this.lblFwdAssoc.Text = "Fwd Assoc:";
			// 
			// btnAssociateRecords
			// 
			this.btnAssociateRecords.Location = new System.Drawing.Point(803, 239);
			this.btnAssociateRecords.Name = "btnAssociateRecords";
			this.btnAssociateRecords.Size = new System.Drawing.Size(75, 23);
			this.btnAssociateRecords.TabIndex = 14;
			this.btnAssociateRecords.Text = "Associate";
			this.btnAssociateRecords.UseVisualStyleBackColor = true;
			this.btnAssociateRecords.Click += new System.EventHandler(this.btnAssociateRecords_Click);
			// 
			// dgvAssociationData
			// 
			this.dgvAssociationData.AllowUserToAddRows = false;
			this.dgvAssociationData.AllowUserToDeleteRows = false;
			this.dgvAssociationData.AllowUserToResizeRows = false;
			this.dgvAssociationData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAssociationData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssociationData.Location = new System.Drawing.Point(391, 288);
			this.dgvAssociationData.MultiSelect = false;
			this.dgvAssociationData.Name = "dgvAssociationData";
			this.dgvAssociationData.ReadOnly = true;
			this.dgvAssociationData.RowHeadersVisible = false;
			this.dgvAssociationData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAssociationData.Size = new System.Drawing.Size(487, 206);
			this.dgvAssociationData.TabIndex = 13;
			// 
			// lblAssociatedData
			// 
			this.lblAssociatedData.AutoSize = true;
			this.lblAssociatedData.Location = new System.Drawing.Point(388, 268);
			this.lblAssociatedData.Name = "lblAssociatedData";
			this.lblAssociatedData.Size = new System.Drawing.Size(101, 15);
			this.lblAssociatedData.TabIndex = 12;
			this.lblAssociatedData.Text = "Association Data:";
			// 
			// btnCreate
			// 
			this.btnCreate.Location = new System.Drawing.Point(21, 501);
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
			this.dgvAssociations.Location = new System.Drawing.Point(21, 288);
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
			this.label3.Location = new System.Drawing.Point(18, 269);
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
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1355, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoadSchema,
            this.mnuImportSchema,
            this.mnuSaveSchema,
            this.toolStripMenuItem1,
            this.mnuExit});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// mnuLoadSchema
			// 
			this.mnuLoadSchema.Name = "mnuLoadSchema";
			this.mnuLoadSchema.Size = new System.Drawing.Size(164, 22);
			this.mnuLoadSchema.Text = "&Load Schema";
			this.mnuLoadSchema.Click += new System.EventHandler(this.mnuLoadSchema_Click);
			// 
			// mnuImportSchema
			// 
			this.mnuImportSchema.Name = "mnuImportSchema";
			this.mnuImportSchema.Size = new System.Drawing.Size(164, 22);
			this.mnuImportSchema.Text = "&Import Schema...";
			this.mnuImportSchema.Click += new System.EventHandler(this.mnuImportSchema_Click);
			// 
			// mnuSaveSchema
			// 
			this.mnuSaveSchema.Name = "mnuSaveSchema";
			this.mnuSaveSchema.Size = new System.Drawing.Size(164, 22);
			this.mnuSaveSchema.Text = "&Save Schema";
			this.mnuSaveSchema.Click += new System.EventHandler(this.mnuSaveSchema_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
			// 
			// mnuExit
			// 
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.Size = new System.Drawing.Size(164, 22);
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// btnMainView
			// 
			this.btnMainView.Location = new System.Drawing.Point(753, 501);
			this.btnMainView.Name = "btnMainView";
			this.btnMainView.Size = new System.Drawing.Size(125, 23);
			this.btnMainView.TabIndex = 21;
			this.btnMainView.Text = "Move to Main View";
			this.btnMainView.UseVisualStyleBackColor = true;
			// 
			// SemanticDesigner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1355, 732);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "SemanticDesigner";
			this.Text = "Semantic Designer";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociationData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssociations)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private System.Windows.Forms.TextBox tbRevAssoc;
		private System.Windows.Forms.TextBox tbFwdAssoc;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblFwdAssoc;
		private System.Windows.Forms.GroupBox gbNavigate;
		private System.Windows.Forms.Button btnDeleteAssoc;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuLoadSchema;
		private System.Windows.Forms.ToolStripMenuItem mnuSaveSchema;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem mnuExit;
		private System.Windows.Forms.ToolStripMenuItem mnuImportSchema;
		private System.Windows.Forms.Button btnMainView;
	}
}

