using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using KSPModAdmin.Core;
using KSPModAdmin.Core.Controller;
using KSPModAdmin.Core.Utils;
using KSPModAdmin.Core.Utils.Localization;
using KSPModAdmin.Plugin.LaunchOptions.Views;

namespace KSPModAdmin.Plugin.LaunchOptions.Controller
{
    /// <summary>
    /// Controller class for the Translation view.
    /// </summary>
    public class PluginViewController
    {
        /// <summary>
        /// Gets or sets the view of the controller.
        /// </summary>
        public static ucPluginView View { get; protected set; }

        internal static void Initialize(ucPluginView view)
        {
            View = view;

            EventDistributor.AsyncTaskStarted += AsyncTaskStarted;
            EventDistributor.AsyncTaskDone += AsyncTaskDone;

            // Add your stuff to initialize here.
            //MainController.View.UcKSPStartup.Visible = false;
            if (true) // This needs to be option driven...
            {
                var ucLaunchPanel = new Views.ucLaunchPanel();
                ucLaunchPanel.Dock = DockStyle.Fill;

                MainController.View.UcKSPStartup.Controls.Clear();
                MainController.View.UcKSPStartup.Controls.Add(ucLaunchPanel);
            }

        }

        #region EventDistributor callback functions.

        /// <summary>
        /// Callback function for the AsyncTaskStarted event.
        /// Should disable all controls of the BaseView.
        /// </summary>
        protected static void AsyncTaskStarted(object sender)
        {
            View.SetEnabledOfAllControls(false);
        }

        /// <summary>
        /// Callback function for the AsyncTaskDone event.
        /// Should enable all controls of the BaseView.
        /// </summary>
        protected static void AsyncTaskDone(object sender)
        {
            View.SetEnabledOfAllControls(true);
        }

        #endregion
    }
}
