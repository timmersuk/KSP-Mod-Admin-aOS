using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KSPModAdmin.Core;
using KSPModAdmin.Core.Controller;
using KSPModAdmin.Core.Utils;
using KSPModAdmin.Core.Utils.Localization;
using KSPModAdmin.Plugin.LaunchOptions.Views;

namespace KSPModAdmin.Plugin.LaunchOptions.Views
{
    public partial class ucLaunchPanel : UserControl
    {
        public ucLaunchPanel()
        {
            InitializeComponent();
        }

        private void btnSetLaunchOptions_Click(object sender, EventArgs e)
        {
           // MainController.View.TabControl.SelectTab("Launch Options");
        }
    }
}
