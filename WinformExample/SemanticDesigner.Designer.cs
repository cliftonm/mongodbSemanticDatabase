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
			this.tbLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dgvSemanticData = new System.Windows.Forms.DataGridView();
			this.lblSemanticType = new System.Windows.Forms.Label();
			this.dgvCollectionData = new System.Windows.Forms.DataGridView();
			this.lblCollectionName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbPlan = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).BeginInit();
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
			this.splitContainer1.Panel2.Controls.Add(this.tbPlan);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Panel2.Controls.Add(this.tbLog);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
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
			this.dgvSemanticData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(443, 536);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "Plan:";
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
			((System.ComponentModel.ISupportInitialize)(this.dgvSemanticData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCollectionData)).EndInit();
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
	}
}

