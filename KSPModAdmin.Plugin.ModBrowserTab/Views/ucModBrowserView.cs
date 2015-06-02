using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using KSPModAdmin.Core.Views;
using KSPModAdmin.Plugin.ModBrowserTab.Controller;
using KSPModAdmin.Plugin.ModBrowserTab.Properties;

namespace KSPModAdmin.Plugin.ModBrowserTab.Views
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public partial class ucModBrowserView : ucBase
    {
        public IKSPMAModBrowser CurrentModBrowser { get; protected set; }

        public bool ShowProcessing
        {
            get { return tslModBrowserProcessing.Visible; }
            set { tslModBrowserProcessing.Visible = value; }
        }

        /// <summary>
        /// Creates a new instance of the ucTranslationView class.
        /// </summary>
        public ucModBrowserView()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode)
                return;

            ModBrowserViewController.Initialize(this);
        }

        #region Event handling

        private void ucPluginView_Load(object sender, EventArgs e)
        {
            // do View related init here or in the PluginViewController.Initialize(...) methode.
        }

        private void cbModBrowser_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sel = cbModBrowser.SelectedItem as ModBrowserInfo;
            if (sel == null)
                return;

            if (CurrentModBrowser != null)
                CurrentModBrowser.ModBrowserView.Visible = false;

            CurrentModBrowser = sel.ModBrowser;
            CurrentModBrowser.ModBrowserView.Visible = true;
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
            return Messages.MSG_MODBROWSER_VIEW_TITLE;
        }

        /// <summary>
        /// Sets the enabled state of some view controls.
        /// </summary>
        public void SetEnabledOfAllControls(bool enable)
        {
            // Enable/Disable your View Controls here.
            // Normally when KSP MA calls this methode with enable = false, all controls should be disabled.
            ////this.Enabled = enable;
            cbModBrowser.Enabled = enable;
        }

        public void ShowProgressBar(bool show, int value, int max = 100, int min = 0)
        {
            InvokeIfRequired(() =>
                {
                    tspbModBrowserProgressBar.Visible = show;

                    if (!show)
                        return;

                    tspbModBrowserProgressBar.Maximum = max;
                    if (max > min)
                        tspbModBrowserProgressBar.Minimum = min;
                    if (value >= min && value <= max)
                        tspbModBrowserProgressBar.Value = value;
                });
        }

        internal void AddModBrowser(IKSPMAModBrowser modBrowser)
        {
            if (!cbModBrowser.Items.Cast<ModBrowserInfo>().Any(mb => mb.ModBrowserName == modBrowser.ModBrowserName))
            {
                cbModBrowser.Items.Add(new ModBrowserInfo(modBrowser));
                Controls.Add(modBrowser.ModBrowserView);
                modBrowser.ModBrowserView.Dock = DockStyle.Fill;
                modBrowser.ModBrowserView.BringToFront();
                modBrowser.ModBrowserView.Visible = false;
            }
        }

        internal void RemoveAllModBrowser()
        {
            cbModBrowser.Items.Clear();
            CurrentModBrowser = null;
        }

        internal void RemoveModBrowser(IKSPMAModBrowser modBrowser)
        {
            RemoveModBrowserByName(modBrowser.ModBrowserName);
        }

        internal void RemoveModBrowserByName(string modBrowserName)
        {
            if (string.IsNullOrEmpty(modBrowserName))
                return;

            var mbInfo = cbModBrowser.Items.Cast<ModBrowserInfo>().FirstOrDefault(x => x.ModBrowserName == modBrowserName);
            if (mbInfo == null)
                return;

            if (CurrentModBrowser != null && mbInfo.ModBrowser == CurrentModBrowser)
            {
                CurrentModBrowser = null;
                cbModBrowser.SelectedItem = null;
            }

            cbModBrowser.Items.Remove(mbInfo);
        }

        #region internal classes

        protected class ModBrowserInfo
        {
            public IKSPMAModBrowser ModBrowser { get; set; }
            public string ModBrowserName { get { return ModBrowser == null ? string.Empty : ModBrowser.ModBrowserName; } }

            public ModBrowserInfo(IKSPMAModBrowser modBrowser)
            {
                ModBrowser = modBrowser;
            }

            public override string ToString()
            {
                return ModBrowser.ModBrowserName;
            }
        }

        #endregion
    }
}
