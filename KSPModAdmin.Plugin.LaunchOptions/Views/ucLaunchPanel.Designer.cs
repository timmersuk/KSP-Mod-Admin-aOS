namespace KSPModAdmin.Plugin.LaunchOptions.Views
{
    partial class ucLaunchPanel
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
            this.tblLayoutLaunchControl = new System.Windows.Forms.TableLayoutPanel();
            this.btnLaunchKSP = new System.Windows.Forms.Button();
            this.btnSetLaunchOptions = new System.Windows.Forms.Button();
            this.tblLayoutLaunchControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblLayoutLaunchControl
            // 
            this.tblLayoutLaunchControl.ColumnCount = 1;
            this.tblLayoutLaunchControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLayoutLaunchControl.Controls.Add(this.btnLaunchKSP, 0, 0);
            this.tblLayoutLaunchControl.Controls.Add(this.btnSetLaunchOptions, 0, 1);
            this.tblLayoutLaunchControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutLaunchControl.Location = new System.Drawing.Point(0, 0);
            this.tblLayoutLaunchControl.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tblLayoutLaunchControl.Name = "tblLayoutLaunchControl";
            this.tblLayoutLaunchControl.RowCount = 2;
            this.tblLayoutLaunchControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tblLayoutLaunchControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tblLayoutLaunchControl.Size = new System.Drawing.Size(664, 66);
            this.tblLayoutLaunchControl.TabIndex = 0;
            // 
            // btnLaunchKSP
            // 
            this.btnLaunchKSP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLaunchKSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLaunchKSP.Image = global::KSPModAdmin.Plugin.LaunchOptions.Properties.Resources.kerbal_24x24;
            this.btnLaunchKSP.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLaunchKSP.Location = new System.Drawing.Point(3, 0);
            this.btnLaunchKSP.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.btnLaunchKSP.Name = "btnLaunchKSP";
            this.btnLaunchKSP.Size = new System.Drawing.Size(658, 36);
            this.btnLaunchKSP.TabIndex = 0;
            this.btnLaunchKSP.Text = "Launch Kerbal Space Program";
            this.btnLaunchKSP.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLaunchKSP.UseVisualStyleBackColor = true;
            // 
            // btnSetLaunchOptions
            // 
            this.btnSetLaunchOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnSetLaunchOptions.Location = new System.Drawing.Point(257, 39);
            this.btnSetLaunchOptions.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnSetLaunchOptions.Name = "btnSetLaunchOptions";
            this.btnSetLaunchOptions.Size = new System.Drawing.Size(150, 27);
            this.btnSetLaunchOptions.TabIndex = 1;
            this.btnSetLaunchOptions.Text = "Set Launch Options";
            this.btnSetLaunchOptions.UseVisualStyleBackColor = true;
            this.btnSetLaunchOptions.Click += new System.EventHandler(this.btnSetLaunchOptions_Click);
            // 
            // ucLaunchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLayoutLaunchControl);
            this.Name = "ucLaunchPanel";
            this.Size = new System.Drawing.Size(664, 66);
            this.tblLayoutLaunchControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayoutLaunchControl;
        public System.Windows.Forms.Button btnLaunchKSP;
        private System.Windows.Forms.Button btnSetLaunchOptions;
    }
}
