using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using KSPModAdmin.Core;
using KSPModAdmin.Core.Controller;
using KSPModAdmin.Core.Utils;
using KSPMODAdmin.Core.Utils.Ckan;
using KSPModAdmin.Core.Utils.Localization;
using KSPModAdmin.Plugin.ModBrowserTab.Model;
using KSPModAdmin.Plugin.ModBrowserTab.Views;

namespace KSPModAdmin.Plugin.ModBrowserTab.Controller
{
    /// <summary>
    /// Controller class for the Translation view.
    /// </summary>
    public class ModBrowserCKANController
    {
        #region Mamber

        private static ModBrowserCKANController instance = null;
        private static CkanTreeModel model = new CkanTreeModel();
        private static Dictionary<string, CkanArchive> archives = new Dictionary<string, CkanArchive>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton of this class.
        /// </summary>
        protected static ModBrowserCKANController Instance
        {
            get { return instance ?? (instance = new ModBrowserCKANController()); }
        }

        /// <summary>
        /// Gets or sets the view of the controller.
        /// </summary>
        public static ucModBrowserCKAN View { get; protected set; }

        #endregion

        internal static void Initialize(ucModBrowserCKAN view)
        {
            View = view;

            EventDistributor.AsyncTaskStarted += AsyncTaskStarted;
            EventDistributor.AsyncTaskDone += AsyncTaskDone;
            EventDistributor.LanguageChanged += LanguageChanged;

            // Add your stuff to initialize here.
            View.Model = model;
            RefreshCkanRepositories();
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

        /// <summary>
        /// Callback function for the LanguageChanged event.
        /// This is the place where you can translate non accessible controls.
        /// </summary>
        protected static void LanguageChanged(object sender)
        {
            View.LanguageChanged();
        }

        #endregion

        public static void RefreshCkanRepositories()
        {
            try
            {
                View.Repositories = CkanRepoManager.GetRepositoryList(CkanRepoManager.MasterRepoListURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public static void RefreshCkanArchive(CkanRepository sel, bool forceDownload = false)
        {
            var parent = View.Parent as ucModBrowserView;
            if (parent != null)
                parent.ShowProcessing = true;
            EventDistributor.InvokeAsyncTaskStarted(Instance);

            model.Nodes.Clear();

            AsyncTask<CkanTreeModel>.DoWork(() =>
                {
                    CkanArchive archive = null;
                    if (!forceDownload && archives.ContainsKey(sel.name))
                        archive = archives[sel.name];
                    else
                    {
                        var fullpath = string.Format("{0}_{1}", sel.name, Path.GetFileName(sel.uri.AbsolutePath));
                        if (!forceDownload && File.Exists(fullpath))
                            archive = CkanRepoManager.CreateRepositoryArchive(fullpath);
                        else
                            archive = CkanRepoManager.GetRepositoryArchive(sel, fullpath);
                        archive.Repository = sel;

                        if (archives.ContainsKey(sel.name))
                            archives[sel.name] = archive;
                        else
                            archives.Add(sel.name, archive);
                    }

                    var newModel = new CkanTreeModel();
                    newModel.AddArchive(archive);

                    return newModel;
                },
                (newModel, ex) =>
                {
                    if (parent != null)
                        parent.ShowProcessing = false;
                    EventDistributor.InvokeAsyncTaskDone(Instance);
                    if (ex != null)
                        MessageBox.Show(ex.Message, "Error");
                    else
                    {
                        model = newModel;
                        View.Model = model;
                    }
                });
        }
    }
}
