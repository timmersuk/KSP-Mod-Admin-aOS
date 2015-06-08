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
            // ucPluginView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUseLaunchOptions);
            this.MinimumSize = new System.Drawing.Size(450, 400);
            this.Name = "ucPluginView";
            this.Size = new System.Drawing.Size(514, 400);
            this.Load += new System.EventHandler(this.ucPluginView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip ttPlugin;
        private System.Windows.Forms.CheckBox chkUseLaunchOptions;
    }
}
