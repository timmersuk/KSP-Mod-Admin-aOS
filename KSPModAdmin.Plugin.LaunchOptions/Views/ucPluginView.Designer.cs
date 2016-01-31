namespace KSPModAdmin.Plugin.LaunchOptions.Views
{
    partial class ucPluginView
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
                Properties.settings.Default.Save();
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
            this.ttPlugin = new System.Windows.Forms.ToolTip(this.components);
            this.chkUseLaunchOptions = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpBasicSettings = new System.Windows.Forms.GroupBox();
            this.labelWindowMode = new System.Windows.Forms.Label();
            this.comboWindowMode = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpBasicSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkUseLaunchOptions
            // 
            this.chkUseLaunchOptions.AutoSize = true;
            this.chkUseLaunchOptions.Location = new System.Drawing.Point(3, 3);
            this.chkUseLaunchOptions.Name = "chkUseLaunchOptions";
            this.chkUseLaunchOptions.Size = new System.Drawing.Size(166, 17);
            this.chkUseLaunchOptions.TabIndex = 0;
            this.chkUseLaunchOptions.Text = "Use extended launch settings";
            this.chkUseLaunchOptions.UseVisualStyleBackColor = true;
            this.chkUseLaunchOptions.CheckedChanged += new System.EventHandler(this.chkUseLaunchOptions_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkUseLaunchOptions, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpBasicSettings, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 400);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // grpBasicSettings
            // 
            this.grpBasicSettings.Controls.Add(this.labelWindowMode);
            this.grpBasicSettings.Controls.Add(this.comboWindowMode);
            this.grpBasicSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBasicSettings.Location = new System.Drawing.Point(3, 26);
            this.grpBasicSettings.Name = "grpBasicSettings";
            this.grpBasicSettings.Size = new System.Drawing.Size(508, 100);
            this.grpBasicSettings.TabIndex = 1;
            this.grpBasicSettings.TabStop = false;
            this.grpBasicSettings.Text = "Basic Settings";
            this.grpBasicSettings.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // labelWindowMode
            // 
            this.labelWindowMode.AutoSize = true;
            this.labelWindowMode.Location = new System.Drawing.Point(6, 16);
            this.labelWindowMode.Name = "labelWindowMode";
            this.labelWindowMode.Size = new System.Drawing.Size(79, 13);
            this.labelWindowMode.TabIndex = 1;
            this.labelWindowMode.Text = "Window Mode:";
            // 
            // comboWindowMode
            // 
            this.comboWindowMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWindowMode.FormattingEnabled = true;
            this.comboWindowMode.Items.AddRange(new object[] {
            "Full Screen",
            "Windowed",
            "Windowed (Borderless)"});
            this.comboWindowMode.Location = new System.Drawing.Point(91, 13);
            this.comboWindowMode.Name = "comboWindowMode";
            this.comboWindowMode.Size = new System.Drawing.Size(158, 21);
            this.comboWindowMode.TabIndex = 0;
            
            // 
            // ucPluginView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(450, 400);
            this.Name = "ucPluginView";
            this.Size = new System.Drawing.Size(514, 400);
            this.Load += new System.EventHandler(this.ucPluginView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpBasicSettings.ResumeLayout(false);
            this.grpBasicSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ttPlugin;
        private System.Windows.Forms.CheckBox chkUseLaunchOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpBasicSettings;
        private System.Windows.Forms.Label labelWindowMode;
        private System.Windows.Forms.ComboBox comboWindowMode;
    }
}
