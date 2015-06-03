using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KSPModAdmin.Core;
using KSPModAdmin.Core.Utils;
using KSPMODAdmin.Core.Utils.Ckan;
using KSPModAdmin.Plugin.ModBrowserTab.Model;
using KSPModAdmin.Plugin.ModBrowserTab.Views;
using KSPModAdmin.Core.Controller;

namespace KSPModAdmin.Plugin.ModBrowserTab.Controller
{
    /// <summary>
    /// Controller class for the Translation view.
    /// </summary>
    public class ModBrowserCKANController
    {
        #region Mamber

        private const string CkanArchiveFolder = "CKAN_Archives";
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

        /// <summary>
        /// Downloads the CKAN Repositories from CkanRepoManager.MasterRepoListURL.
        /// And updates the View.
        /// </summary>
        /// <param name="finishedCallback">Optional callback function. Will be called after finishing the async get.</param>
        public static void RefreshCkanRepositories(Action finishedCallback = null)
        {
            var parent = View.Parent as ucModBrowserView;
            if (parent != null)
                parent.ShowProcessing = true;

            Messenger.AddInfo(Messages.MSG_REFRESHING_REPOSITORIES);
            EventDistributor.InvokeAsyncTaskStarted(Instance);
            AsyncTask<CkanRepositories>.DoWork(() =>
                {
                    return CkanRepoManager.GetRepositoryList(CkanRepoManager.MasterRepoListURL);
                },
                (reulst, ex) =>
                {
                    EventDistributor.InvokeAsyncTaskDone(Instance);

                    if (parent != null)
                        parent.ShowProcessing = false;

                    if (ex != null)
                        Messenger.AddError(string.Format(Messages.MSG_ERROR_DURING_REFRESH_REPOSITORIES_0, ex.Message), ex);
                    else
                    { 
                        CkanRepository last = View.SelectedRepository;
                        View.Repositories = CkanRepoManager.GetRepositoryList(CkanRepoManager.MasterRepoListURL);
                        View.SelectedRepository = last;
                    }

                    Messenger.AddInfo(Messages.MSG_REFRESHING_REPOSITORIES_DONE);

                    if (finishedCallback != null)
                        finishedCallback();
                });
        }

        /// <summary>
        /// Downloads the Ckan Repository archive if necessary, creates a CkanArchive from it and populates the view.
        /// </summary>
        /// <param name="repo">The Ckan Repository to get the Archive for.</param>
        /// <param name="forceDownload">If false the download will be skipped if a Ckan Repository archive file already exists.</param>
        /// <param name="finishedCallback">Optional callback function. Will be called after finishing the async get.</param>
        public static void RefreshCkanArchive(CkanRepository repo, bool forceDownload = false, Action finishedCallback = null)
        {
            model.Nodes.Clear();

            if (repo == null) 
                return;

            if (!OptionsController.HasValidDownloadPath)
            {
                Messenger.AddInfo(Messages.MSG_DOWNLOADPATH_MISSING);
                OptionsController.SelectNewDownloadPath();
                if (!OptionsController.HasValidDownloadPath)
                    return;
            }

            var parent = View.Parent as ucModBrowserView;
            if (parent != null)
                parent.ShowProcessing = true;
            EventDistributor.InvokeAsyncTaskStarted(Instance);
            Messenger.AddInfo(string.Format(Messages.MSG_REFRESHING_REPOSITORY_ARCHIVE_0, repo.name));

            AsyncTask<CkanArchive>.DoWork(() =>
                {
                    CkanArchive archive = null;
                    if (!forceDownload && archives.ContainsKey(repo.name))
                    {
                        Messenger.AddInfo(Messages.MSG_USING_CACHED_ARCHIVE);
                        archive = archives[repo.name];
                    }
                    else
                    {
                        var path = Path.Combine(OptionsController.DownloadPath, CkanArchiveFolder);
                        if (!Directory.Exists(path))
                        {
                            Messenger.AddInfo(Messages.MSG_CREATE_CKAN_ARCHIVE);
                            Directory.CreateDirectory(path);
                        }

                        var filename = string.Format("{0}_{1}", repo.name, Path.GetFileName(repo.uri.AbsolutePath));
                        var fullpath = Path.Combine(path, filename);
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

                    return archive;
                },
                (newArchive, ex) =>
                {
                    if (parent != null)
                        parent.ShowProcessing = false;
                    EventDistributor.InvokeAsyncTaskDone(Instance);
                    if (ex != null)
                    {
                        Messenger.AddError(string.Format(Messages.MSG_ERROR_DURING_REFRESH_REPOSITORY_ARCHIVE_0, ex.Message), ex);
                    }
                    else
                    {
                        model.AddArchive(newArchive);
                        View.CountLabelText = string.Format(Messages.MSG_MODBROWSER_CKAN_COUNT_TEXT, newArchive.Mods.Count, model.Nodes.Count);
                    }

                    Messenger.AddInfo(Messages.MSG_REFRESH_REPOSITORY_DONE);

                    if (finishedCallback != null)
                        finishedCallback();
                });
        }

        /// <summary>
        /// Processes all changes mods
        /// Installs or uninstalls them.
        /// </summary>
        public static void ProcessChanges()
        {
            Messenger.AddInfo(Messages.MSG_PROCESSING_STARTED);

            foreach (CkanNode modInfo in model.Nodes.Cast<CkanNode>().SelectMany(mod => mod.Nodes.Cast<CkanNode>().Where(modInfo => modInfo.Added != modInfo.Checked)))
            {
                // TODO: Do the real work
                modInfo.Added = modInfo.Checked;
            }

            Messenger.AddInfo(Messages.MSG_PROCESSING_DONE);

            View.InvalidateView();
        }
    }
}
