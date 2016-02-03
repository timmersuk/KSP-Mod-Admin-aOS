using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using KSPModAdmin.Core.Views;
using KSPModAdmin.Plugin.LaunchOptions.Controller;
using KSPModAdmin.Plugin.LaunchOptions.Properties;

namespace KSPModAdmin.Plugin.LaunchOptions.Views
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public partial class ucPluginView : ucBase
    {
        /// <summary>
        /// Creates a new instance of the ucPluginView class.
        /// </summary>
        public ucPluginView()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode)
                return;

            PluginViewController.Initialize(this);
            chkUseLaunchOptions.Checked = Properties.settings.Default.useLaunchOptions;
            if (Properties.settings.Default.fullScreen)
            {
                comboWindowMode.SelectedText = "Fullscreen";
            }
            else
            {
                if (Properties.settings.Default.windowBorderless)
                {
                    comboWindowMode.SelectedText = "Windowed (Borderless)";
                }
                else
                {
                    comboWindowMode.SelectedText = "Windowed";
                }
            }
        }

        #region Event handling

        private void ucPluginView_Load(object sender, EventArgs e)
        {
            // do View related init here or in the PluginViewController.Initialize(...) methode.
        }

        #endregion

        /// <summary>
        /// Forces the view to redraw.
        /// </summary>
        public override void InvalidateView()
        {
            Invalidate();
            Update();
            Refresh();
        }

        /// <summary>
        /// Gets the Name for the parent TabPage.
        /// </summary>
        /// <returns>The Name for the parent TabPage.</returns>
        public override string GetTabCaption()
        {
            return Messages.MSG_LAUNCHOPTIONS_VIEW_TITLE;
        }

        /// <summary>
        /// Sets the enabled state of some view controls.
        /// </summary>
        public void SetEnabledOfAllControls(bool enable)
        {
            // Enable/Disable your View Controls here.
            // Normally when KSP MA calls this methode with enable = false, all controls should be disabled.
            this.Enabled = enable;
        }

        private void chkUseLaunchOptions_CheckedChanged(object sender, EventArgs e)
        {
           if (chkUseLaunchOptions.Checked)
           {
               PluginViewController.EnableLaunchOptions();
               Properties.settings.Default.useLaunchOptions = true;
               Properties.settings.Default.Save();
           }
           else
           {
               PluginViewController.EnableLaunchOptions(false);
               Properties.settings.Default.useLaunchOptions = false;
               Properties.settings.Default.Save();
           }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}
