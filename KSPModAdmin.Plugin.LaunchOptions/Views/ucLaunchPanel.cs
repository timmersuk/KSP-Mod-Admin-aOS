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
            //// Debug code: list all tabs by name to debug log
            //var tabs = ",";
            //foreach (TabPage tab in MainController.View.TabControl.TabPages)
            //{
            //    tabs = tabs + "," + tab.Name;
            //}
            //Messenger.AddDebug("tabs are:" + tabs);

            MainController.View.TabControl.SelectTab("b1ed6a13-2e8c-4d1b-8012-a90af9b24c3e");
        }
    }
}
