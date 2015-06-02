using System.Windows.Forms;
using KSPModAdmin.Plugin.ModBrowserTab.Views;

namespace KSPModAdmin.Plugin.ModBrowserTab
{
    public interface IKSPMAModBrowser
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string ModBrowserName { get; }

        /// <summary>
        /// Short description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// View which should be added to the ModBrowser.
        /// </summary>
        UserControl ModBrowserView { get; }
    }
}