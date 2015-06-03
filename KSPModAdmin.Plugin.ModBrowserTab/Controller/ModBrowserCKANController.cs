using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KSPModAdmin.Core;
using KSPMODAdmin.Core.Utils.Ckan;
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
                CkanRepository last = View.SelectedRepository;
                View.Repositories = CkanRepoManager.GetRepositoryList(CkanRepoManager.MasterRepoListURL);
                View.SelectedRepository = last;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public static void RefreshCkanArchive(CkanRepository repo, bool forceDownload = false)
        {
            model.Nodes.Clear();

            if (repo == null) 
                return;

            var parent = View.Parent as ucModBrowserView;
            if (parent != null)
                parent.ShowProcessing = true;
            EventDistributor.InvokeAsyncTaskStarted(Instance);

            CkanArchive archive = null;
            AsyncTask<CkanTreeModel>.DoWork(() =>
                {
                    if (!forceDownload && archives.ContainsKey(repo.name))
                        archive = archives[repo.name];
                    else
                    {
                        var fullpath = string.Format("{0}_{1}", repo.name, Path.GetFileName(repo.uri.AbsolutePath));
                        if (!forceDownload && File.Exists(fullpath))
                            archive = CkanRepoManager.CreateRepositoryArchive(fullpath);
                        else
                            archive = CkanRepoManager.GetRepositoryArchive(repo, fullpath);
                        archive.Repository = repo;

                        if (archives.ContainsKey(repo.name))
                            archives[repo.name] = archive;
                        else
                            archives.Add(repo.name, archive);
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
                        View.CountLabelText = string.Format(Messages.MSG_MODBROWSER_CKAN_COUNT_TEXT, archive.Mods.Count, model.Nodes.Count);
                    }
                });
        }
    }
}
