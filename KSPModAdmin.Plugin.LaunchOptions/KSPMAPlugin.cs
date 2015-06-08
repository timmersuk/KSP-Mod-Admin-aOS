using System;
using System.Collections.Generic;
using KSPModAdmin.Core;
using KSPModAdmin.Core.Utils;
using KSPModAdmin.Plugin.LaunchOptions.Properties;
using KSPModAdmin.Plugin.LaunchOptions.Views;

namespace KSPModAdmin.Plugin.LaunchOptions
{
    /// <summary>
    /// KSP MA plugin container class.
    /// </summary>
    public class KSPMAPlugin : IKSPMAPlugin
    {
        private TabView[] mMainTabViews = null;
        private TabView[] mOptionTabViews = new List<TabView>().ToArray(); // Initialize with an empty array if not needed.


        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name { get { return "Launch Options Plugin"; } }

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description
        {
            get
            {
                return "This Plugin adds a tab to fine-tune the launch options for KSP.";
            }
        }

        /// <summary>
        /// Array of TabViews that the mod imports.
        /// </summary>
        public TabView[] MainTabViews
        {
            get
            {
                if (mMainTabViews == null)
                    mMainTabViews = new[] { new TabView(new Guid("{B1ED6A13-2E8C-4D1B-8012-A90AF9B24C3E}"), new ucPluginView(), Resources.cube_molecule) };

                return mMainTabViews;
            }
        }

        /// <summary>
        /// Array of OptionTabViews that the mod imports.
        /// </summary>
        public TabView[] OptionTabViews
        {
            get
            {
                return mOptionTabViews;
            }
        }
    }
}
