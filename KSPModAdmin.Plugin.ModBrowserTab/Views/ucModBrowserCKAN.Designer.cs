namespace KSPModAdmin.Plugin.ModBrowserTab.Views
{
    partial class ucModBrowserCKAN
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tsModBrowserCkan = new System.Windows.Forms.ToolStrip();
            this.tslModBrowserCkanRepository = new System.Windows.Forms.ToolStripLabel();
            this.cbModBrowserCkanRepository = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbModBrowserCkanRefresh = new System.Windows.Forms.ToolStripButton();
            this.tvCkanRepositories = new KSPModAdmin.Core.Utils.Controls.Aga.Controls.Tree.TreeViewAdv();
            this.lblModBrowserCkanCount = new System.Windows.Forms.Label();
            this.ttModBrowserCkan = new System.Windows.Forms.ToolTip(this.components);
            this.tsModBrowserCkan.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsModBrowserCkan
            // 
            this.tsModBrowserCkan.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslModBrowserCkanRepository,
            this.cbModBrowserCkanRepository,
            this.toolStripSeparator1,
            this.tsbModBrowserCkanRefresh});
            this.tsModBrowserCkan.Location = new System.Drawing.Point(0, 0);
            this.tsModBrowserCkan.Name = "tsModBrowserCkan";
            this.tsModBrowserCkan.Size = new System.Drawing.Size(675, 31);
            this.tsModBrowserCkan.TabIndex = 0;
            this.tsModBrowserCkan.Text = "toolStrip1";
            // 
            // tslModBrowserCkanRepository
            // 
            this.tslModBrowserCkanRepository.Name = "tslModBrowserCkanRepository";
            this.tslModBrowserCkanRepository.Size = new System.Drawing.Size(66, 28);
            this.tslModBrowserCkanRepository.Text = "Repository:";
            // 
            // cbModBrowserCkanRepository
            // 
            this.cbModBrowserCkanRepository.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModBrowserCkanRepository.Name = "cbModBrowserCkanRepository";
            this.cbModBrowserCkanRepository.Size = new System.Drawing.Size(150, 31);
            this.cbModBrowserCkanRepository.DropDown += new System.EventHandler(this.cbModBrowserCkanRepository_DropDown);
            this.cbModBrowserCkanRepository.SelectedIndexChanged += new System.EventHandler(this.cbModBrowserCkanRepository_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbModBrowserCkanRefresh
            // 
            this.tsbModBrowserCkanRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbModBrowserCkanRefresh.Image = global::KSPModAdmin.Plugin.ModBrowserTab.Properties.Resources.refresh_24x24;
            this.tsbModBrowserCkanRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbModBrowserCkanRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbModBrowserCkanRefresh.Name = "tsbModBrowserCkanRefresh";
            this.tsbModBrowserCkanRefresh.Size = new System.Drawing.Size(28, 28);
            this.tsbModBrowserCkanRefresh.Text = "toolStripButton1";
            this.tsbModBrowserCkanRefresh.Click += new System.EventHandler(this.cbModBrowserCkanRepository_SelectedIndexChanged);
            // 
            // tvCkanRepositories
            // 
            this.tvCkanRepositories.AllowColumnSort = true;
            this.tvCkanRepositories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvCkanRepositories.BackColor = System.Drawing.SystemColors.Window;
            this.tvCkanRepositories.DefaultToolTipProvider = null;
            this.tvCkanRepositories.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvCkanRepositories.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvCkanRepositories.Location = new System.Drawing.Point(0, 31);
            this.tvCkanRepositories.Model = null;
            this.tvCkanRepositories.Name = "tvCkanRepositories";
            this.tvCkanRepositories.SelectedNode = null;
            this.tvCkanRepositories.Size = new System.Drawing.Size(675, 389);
            this.tvCkanRepositories.TabIndex = 1;
            this.tvCkanRepositories.Text = "treeViewAdv1";
            this.tvCkanRepositories.UseColumns = true;
            // 
            // lblModBrowserCkanCount
            // 
            this.lblModBrowserCkanCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblModBrowserCkanCount.AutoSize = true;
            this.lblModBrowserCkanCount.Location = new System.Drawing.Point(3, 426);
            this.lblModBrowserCkanCount.Name = "lblModBrowserCkanCount";
            this.lblModBrowserCkanCount.Size = new System.Drawing.Size(47, 13);
            this.lblModBrowserCkanCount.TabIndex = 27;
            this.lblModBrowserCkanCount.Text = "Count: 0";
            // 
            // ucModBrowserCKAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblModBrowserCkanCount);
            this.Controls.Add(this.tvCkanRepositories);
            this.Controls.Add(this.tsModBrowserCkan);
            this.Name = "ucModBrowserCKAN";
            this.Size = new System.Drawing.Size(675, 442);
            this.Load += new System.EventHandler(this.ucModBrowserCKAN_Load);
            this.tsModBrowserCkan.ResumeLayout(false);
            this.tsModBrowserCkan.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsModBrowserCkan;
        private System.Windows.Forms.ToolStripLabel tslModBrowserCkanRepository;
        private System.Windows.Forms.ToolStripButton tsbModBrowserCkanRefresh;
        private System.Windows.Forms.ToolStripComboBox cbModBrowserCkanRepository;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Core.Utils.Controls.Aga.Controls.Tree.TreeViewAdv tvCkanRepositories;
        private System.Windows.Forms.Label lblModBrowserCkanCount;
        private System.Windows.Forms.ToolTip ttModBrowserCkan;
    }
}
