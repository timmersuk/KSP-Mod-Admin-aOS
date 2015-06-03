using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using KSPModAdmin.Core.Utils;
using Newtonsoft.Json;
using SharpCompress.Archive;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    /// <summary>
    /// Helper class to manage the Ckan Repositories.
    /// </summary>
    public class CkanRepoManager
    {
        public static readonly Uri DefaultRepoURL = new Uri("https://github.com/KSP-CKAN/CKAN-meta/archive/master.zip");
        public static readonly Uri MasterRepoListURL = new Uri("http://api.ksp-ckan.org/mirrors");
        public static string EXT_RAR = ".rar";

        /// <summary>
        /// Downloads the list of Ckan Repositories from the passed URL.
        /// </summary>
        /// <param name="repoListURL">The URL to get the Ckan Repositories from.</param>
        /// <returns>The list of Ckan Repositories from the passed URL.</returns>
        public static CkanRepositories GetRepositoryList(Uri repoListURL)
        {
            Messenger.AddInfo(string.Format("Downloading repository list from \"{0}\"...", repoListURL.AbsoluteUri));
            
            string content = Www.Load(repoListURL.AbsoluteUri);
            var repos = JsonConvert.DeserializeObject<CkanRepositories>(content);
            
            Messenger.AddInfo(string.Format("Downloading repository list done. {0} repositories found.", repos.repositories.Length));

            return repos;
        }

        /// <summary>
        /// Gets the named Ckan Repository from the Ckan Repository list.
        /// </summary>
        /// <param name="repositories">List of Ckan Repository to search in.</param>
        /// <param name="repoName">Name of the Ckan Repository to look for.</param>
        /// <returns>The named Ckan Repository from the Ckan Repository list or null.</returns>
        public static CkanRepository GetRepository(CkanRepositories repositories, string repoName)
        {
            return repositories.repositories.FirstOrDefault(x => x.name.Equals(repoName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Downloads the Ckan Repository archive to full path.
        /// </summary>
        /// <param name="repo">The Ckan Repository to get the Ckan Repository archive for.</param>
        /// <param name="fullpath">The full path to write the downloaded file to.</param>
        /// <returns>The new created CkanArchive which was constructed from the downloaded Ckan Repository archive.</returns>
        public static CkanArchive GetRepositoryArchive(CkanRepository repo, string fullpath)
        {
            Messenger.AddInfo(string.Format("Downloading repository archive \"{0}\" from \"{1}\"...", repo.name, repo.uri.AbsoluteUri));
            WebClient client = new WebClient();
            client.DownloadFile(repo.uri, fullpath);
            Messenger.AddInfo(string.Format("Downloading repository archive \"{0}\" done.", repo.name));

            return CreateRepositoryArchive(fullpath);
        }

        /// <summary>
        /// Creates a CkanArchive from a Ckan Repository archive file.
        /// </summary>
        /// <param name="fullpath">The full path to the Ckan Repository archive.</param>
        /// <returns>The new created CkanArchive from a Ckan Repository archive file.</returns>
        public static CkanArchive CreateRepositoryArchive(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath) || !File.Exists(fullpath))
                return null;

            Messenger.AddInfo(string.Format("Reading repository archive \"{0}\"...", fullpath));
            var repoArchive = new CkanArchive { FullPath = fullpath };
            using (IArchive archive = ArchiveFactory.Open(repoArchive.FullPath))
            {
                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (Path.GetDirectoryName(entry.FilePath).Split(Path.DirectorySeparatorChar).Length != 2)
                    {
                        Messenger.AddInfo(string.Format("Archive entry \"{0}\" skipped.", entry.FilePath));
                        continue;
                    }

                    if (entry.IsDirectory)
                    {
                        var mod = CreateMod(entry);
                        if (mod != null)
                            repoArchive.Mods.Add(mod.Name, mod);
                    }
                    else
                    {
                        var name = GetDirectoryName(entry.FilePath);
                        if (!repoArchive.Mods.ContainsKey(name))
                            continue;

                        var mod = repoArchive.Mods[name];
                        var modInfo = CreateModInfos(entry);
                        modInfo.Mod = mod;
                        if (modInfo != null)
                            mod.ModInfos.Add(modInfo);
                    }
                }
            }
            Messenger.AddInfo(string.Format("Reading repository archive \"{0}\" done.", fullpath));

            return repoArchive;
        }

        private static CkanMod CreateMod(IArchiveEntry archiveEntry)
        {
            if (!archiveEntry.IsDirectory)
                return null;

            CkanMod mod = new CkanMod();
            mod.ArchivePath = archiveEntry.FilePath;
            mod.Name = GetDirectoryName(archiveEntry.FilePath);

            Messenger.AddInfo(string.Format("Mod \"{0}\" created from \"{1}\"", mod.Name, archiveEntry.FilePath));

            return mod;
        }

        private static CkanModInfo CreateModInfos(IArchiveEntry archiveEntry)
        {
            MemoryStream ms = new MemoryStream();
            archiveEntry.WriteTo(ms);
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var content = sr.ReadToEnd();
            var modInfos = JsonConvert.DeserializeObject<CkanModInfo>(content);
            Messenger.AddInfo(string.Format("ModInfos \"{0}\"-\"{1}\" created from \"{2}\"", modInfos.name, modInfos.version, archiveEntry.FilePath));
            return modInfos;
        }

        private static string GetDirectoryName(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                return string.Empty;

            string[] dirs = Path.GetDirectoryName(dirPath).Split(Path.DirectorySeparatorChar);
            return dirs.Length > 0 ? dirs[dirs.Length - 1] : string.Empty;
        }
    }
}