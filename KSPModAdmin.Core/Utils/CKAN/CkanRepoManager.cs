using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using SharpCompress.Archive;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanRepoManager
    {
        public static readonly Uri DefaultRepoURL = new Uri("https://github.com/KSP-CKAN/CKAN-meta/archive/master.zip");
        public static readonly Uri MasterRepoListURL = new Uri("http://api.ksp-ckan.org/mirrors");
        public static string EXT_RAR = ".rar";

        public static CkanRepositories GetRepositoryList(Uri repoListURL)
        {
            WebClient client = new WebClient();
            string content = client.DownloadString(repoListURL);
            return JsonConvert.DeserializeObject<CkanRepositories>(content);
        }

        public static CkanRepository GetRepository(CkanRepositories repositories, string repoName)
        {
            return repositories.repositories.FirstOrDefault(x => x.name.Equals(repoName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static CkanArchive GetRepositoryArchive(CkanRepository repo, string fullpath)
        {
            WebClient client = new WebClient();
            client.DownloadFile(repo.uri, fullpath);

            return CreateRepositoryArchive(fullpath);
        }

        public static CkanArchive CreateRepositoryArchive(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath) || !File.Exists(fullpath))
                return null;

            var repoArchive = new CkanArchive { FullPath = fullpath };
            using (IArchive archive = ArchiveFactory.Open(repoArchive.FullPath))
            {
                // Rar files uses \ as path separator.
                char separator = '/';
                string extension = Path.GetExtension(repoArchive.FullPath);
                if (extension != null && extension.Equals(EXT_RAR, StringComparison.CurrentCultureIgnoreCase))
                    separator = '\\';

                foreach (IArchiveEntry entry in archive.Entries)
                {
                    if (Path.GetDirectoryName(entry.FilePath).Split(Path.DirectorySeparatorChar).Length != 2)
                        continue;

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

            return repoArchive;
        }

        private static CkanMod CreateMod(IArchiveEntry archiveEntry)
        {
            if (!archiveEntry.IsDirectory)
                return null;

            CkanMod mod = new CkanMod();
            mod.ArchivePath = archiveEntry.FilePath;
            mod.Name = GetDirectoryName(archiveEntry.FilePath);

            return mod;
        }

        private static CkanModInfo CreateModInfos(IArchiveEntry archiveEntry)
        {
            MemoryStream ms = new MemoryStream();
            archiveEntry.WriteTo(ms);
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var content = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<CkanModInfo>(content);
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