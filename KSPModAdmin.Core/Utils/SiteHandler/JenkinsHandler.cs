using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KSPModAdmin.Core.Controller;
using KSPModAdmin.Core.Model;
using KSPModAdmin.Core.Views;
using System.Threading.Tasks;
using RestSharp;

namespace KSPModAdmin.Core.Utils.SiteHandler
{
    /// <summary>
    /// Handles the GetModInfo and Mod download for mods on Jenkins.
    /// </summary>
    public class JenkinsHandler : ISiteHandler
    {
        private const string NAME = "Jenkins";

        private const string URL_0_1 = "https://{0}/jenkins/job/{1}/";

        /// <summary>
        /// Builds the url from the passed user and project name.
        /// </summary>
        /// <param name="hostname">Jenkins hostname.</param>
        /// <param name="userName">Name of the user from the GitHub repository.</param>
        /// <param name="projectName">Name of the project from the GitHub repository.</param>
        /// <returns>The build GitHub project URL or empty string.</returns>
        public static string GetProjectUrl(string hostname, string projectName)
        {
            if (!string.IsNullOrEmpty(hostname) && !string.IsNullOrEmpty(projectName))
                return string.Format(URL_0_1, hostname, projectName);

            return string.Empty;
        }

        /// <summary>
        /// Gets the Name of the ISiteHandler.
        /// </summary>
        /// <returns>The Name of the ISiteHandler.</returns>
        public string Name { get { return NAME; } }

        /// <summary>
        /// Checks if the passed URL is a valid URL for Jenkins.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>True if the passed URL is a valid URL, otherwise false.</returns>
        public bool IsValidURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            
            UrlParts parts = GetUrlParts(url);

            return (parts != null);
        }

        /// <summary>
        /// Gets the content of the site of the passed URL and parses it for ModInfos.
        /// </summary>
        /// <param name="url">The URL of the site to parse the ModInfos from.</param>
        /// <returns>The ModInfos parsed from the site of the passed URL.</returns>
        public ModInfo GetModInfo(string url)
        {
            UrlParts parts = GetUrlParts(url);

            ModInfo modInfo = new ModInfo
            {
                SiteHandlerName = Name,
                ModURL = ReduceToPlainUrl(url),
                Name = parts.modName.Replace("/", ""),
                Author = parts.target
            };

            ParseSite(ref modInfo);

            return modInfo;
        }

        /// <summary>
        /// Handles a mod add via URL.
        /// Validates the URL, gets ModInfos, downloads mod archive, adds it to the ModSelection and installs the mod if selected.
        /// </summary>
        /// <param name="url">The URL to the mod.</param>
        /// <param name="modName">The name for the mod.</param>
        /// <param name="install">Flag to determine if the mod should be installed after adding.</param>
        /// <param name="downloadProgressCallback">Callback function for download progress.</param>
        /// <returns>The root node of the added mod, or null.</returns>
        public ModNode HandleAdd(string url, string modName, bool install, DownloadProgressCallback downloadProgressCallback = null)
        {
            ModInfo modInfo = GetModInfo(url);
            if (modInfo == null)
                return null;

            if (!string.IsNullOrEmpty(modName))
                modInfo.Name = modName;

            ModNode newMod = null;
            if (DownloadMod(ref modInfo, downloadProgressCallback))
                newMod = ModSelectionController.HandleModAddViaModInfo(modInfo, install);

            return newMod;
        }

        /// <summary>
        /// Checks if updates are available for the passed mod.
        /// </summary>
        /// <param name="modInfo">The ModInfos of the mod to check for updates.</param>
        /// <param name="newModInfo">A reference to an empty ModInfo to write the updated ModInfos to.</param>
        /// <returns>True if there is an update, otherwise false.</returns>
        public bool CheckForUpdates(ModInfo modInfo, ref ModInfo newModInfo)
        {
            newModInfo = GetModInfo(modInfo.ModURL);
            if (string.IsNullOrEmpty(modInfo.Version) && !string.IsNullOrEmpty(newModInfo.Version))
                return true;
            else if (!string.IsNullOrEmpty(modInfo.Version) && !string.IsNullOrEmpty(newModInfo.Version))
                return (VersionComparer.CompareVersions(modInfo.Version, newModInfo.Version) == VersionComparer.Result.AisSmallerB);
            else if (string.IsNullOrEmpty(modInfo.CreationDate) && !string.IsNullOrEmpty(newModInfo.CreationDate))
                return true;
            else if (!string.IsNullOrEmpty(modInfo.CreationDate) && !string.IsNullOrEmpty(newModInfo.CreationDate))
                return modInfo.CreationDateAsDateTime < newModInfo.CreationDateAsDateTime;

            return false;
        }

        /// <summary>
        /// Downloads the mod.
        /// </summary>
        /// <param name="modInfo">The infos of the mod. Must have at least ModURL and LocalPath</param>
        /// <param name="downloadProgressCallback">Callback function for download progress.</param>
        /// <returns>True if the mod was downloaded.</returns>
        public bool DownloadMod(ref ModInfo modInfo, DownloadProgressCallback downloadProgressCallback = null)
        {
            if (modInfo == null)
                return false;

            var downloadInfos = GetDownloadInfo(modInfo);
            DownloadInfo selected = null;

            // If any of the nodes came back as a prerelease, notify the user that there are pre-release nodes
            foreach (var d in downloadInfos)
            {
                if (!d.Name.Contains("Pre-release")) continue;

                var dlg = MessageBox.Show("This download contains a pre-release version. This version might not be stable.", Messages.MSG_TITLE_ATTENTION, MessageBoxButtons.OK);
                break;
            }

            if (downloadInfos.Count > 1)
            {
                // create new selection form if more than one download option found
                var dlg = new frmSelectDownload(downloadInfos);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    selected = dlg.SelectedLink;
                }
            }
            else if (downloadInfos.Count == 1)
            {
                selected = downloadInfos.First();
            }
            else
            {
                string msg = string.Format(Messages.MSG_NO_BINARY_DOWNLOAD_FOUND_AT_0, modInfo.SiteHandlerName);
                MessageBox.Show(msg, Messages.MSG_TITLE_ERROR);
                Messenger.AddDebug(msg);
                return false;
            }

            if (selected != null)
            {
                string downloadUrl = selected.DownloadURL;
                modInfo.LocalPath = Path.Combine(OptionsController.DownloadPath, selected.Filename);
                Www.DownloadFile(downloadUrl, modInfo.LocalPath, downloadProgressCallback);
            }

            return File.Exists(modInfo.LocalPath);
        }
        



        /// <summary>
        /// Takes a site url and parses the site for mod info
        /// </summary>
        /// <param name="modInfo">The modInfo to add data to</param>
        public void ParseSite(ref ModInfo modInfo)
        {
            RestClient client = new RestClient(modInfo.Author);

            //https://ksp.sarbian.com/jenkins/job/GCMonitor/lastSuccessfulBuild/api/xml
            RestRequest request = new RestRequest("/jenkins/job/" + modInfo.Name + "/lastSuccessfulBuild/api/json", Method.GET);

            Build build = GetBuild(client, request);
            if (build == null)
            {
                Messenger.AddError("Error! Unable to find latest jenkins release");
                return;
            }

            if (GetZipArtifact(build) == null)
            { 
                Messenger.AddError("Error! No zip file in artifacts in latest Jenkins release");
                return;
            }

            modInfo.Version = build.id;
            modInfo.ChangeDateAsDateTime = new DateTime(build.timestamp * TimeSpan.TicksPerMillisecond);
        }

        /// <summary>
        /// Takes a Jenkins url and sets it to the shortest path to the project
        /// </summary>
        /// <param name="url">Jenkins project url</param>
        /// <returns>Shortest Jenkins project url</returns>
        public string ReduceToPlainUrl(string url)
        {
            UrlParts parts = GetUrlParts(url);

            return parts.target + "://jenkins/job/" + parts.modName;
        }

        private class UrlParts
        {
            public string target;
            public string modName;
        }

        /// <summary>
        /// Splits a url into it's segment parts
        /// </summary>
        /// <param name="modUrl">A url to split</param>
        /// <exception cref="ArgumentException">ArgumentException("GitHub URL must point to a repository.")</exception>
        /// <returns>An array of the url segments
        /// 1: baseURL - scheme host - e.g. https://ksp.sarbian.com/
        /// 2: Job name
        private static UrlParts GetUrlParts(string modUrl)
        {
            Uri uri = new Uri(modUrl);

            string[] pathSegments = uri.Segments;

            // /jenkins/job/{jobName}
            if (pathSegments == null || 
                pathSegments.Count() < 4 ||
                pathSegments[0] != "/" ||
                pathSegments[1] != "jenkins/" ||
                pathSegments[2] != "job/")
            {
                return null;
            }

            return new UrlParts()
            {
                target = uri.Scheme + "://" + uri.Authority,
                modName = pathSegments[3]
            };
        }



        private Build GetBuild(RestClient client, string modName, string buildNumber)
        {
            RestRequest request = new RestRequest("/jenkins/job/" + modName + "/" + buildNumber + "/api/json", Method.GET);

            Build build = GetBuild(client, request);

            return build;
        }

        private Build GetBuild(RestClient client, RestRequest request)
        {
            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            IRestResponse<Build> response2 = client.Execute<Build>(request);

            Build build = response2.Data;
            if (build == null)
            {
                Messenger.AddError("Error! Can't find build");
                return null;
            }

            if (build.id == null)
            {
                Messenger.AddError("Error! No id in build");
                return null;
            }

            if (build.timestamp == 0)
            {
                Messenger.AddError("Error! No timestamp in build");
                return null;
            }

            return build;
        }

        private Project GetProject(RestClient client, string modName)
        {
            RestRequest request = new RestRequest("/jenkins/job/" + modName + "/api/json", Method.GET);

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            IRestResponse<Project> response2 = client.Execute<Project>(request);
            if (response2 == null)
            {
                return null;
            }

            Project project = response2.Data;

            return project;
        }

        private Artifact GetZipArtifact(Build build)
        {
            if (build.artifacts == null || build.artifacts.Count == 0)
            {
                return null;
            }

            foreach (Artifact artifact in build.artifacts)
            {
                if (artifact.fileName != null && artifact.fileName.EndsWith(".zip"))
                {
                    return artifact;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a list of DownloadInfos from a GitHub release
        /// </summary>
        /// <param name="modInfo">The mod to generate the list from</param>
        /// <returns>A list of one or more DownloadInfos for the most recent release of the selected repository</returns>
        private List<DownloadInfo> GetDownloadInfo(ModInfo modInfo)
        {
            List<DownloadInfo> results = new List<DownloadInfo>();

            RestClient client = new RestClient(modInfo.Author);

            Project project = GetProject(client, modInfo.Name);
            if (project == null)
            {
                Messenger.AddError("Error! Can't find project info");
                return results;
            }

            if (project.builds == null)
            {
                Messenger.AddError("Error! Can't find any builds in project info");
                return results;
            }

            foreach (BuildSummary buildSummary in project.builds)
            {
                if (buildSummary.url == null)
                {
                    continue;
                }

                Uri uri = new Uri(buildSummary.url);
                string[] segments = uri.Segments;
                // last segment should be the build number

                string buildNumber = segments.Last().Replace("/", "");

                Build build = GetBuild(client, modInfo.Name, buildNumber);
                if (build.result != "SUCCESS")
                {
                    continue;
                }

                Artifact artifact = GetZipArtifact(build);

                if (artifact == null)
                {
                    continue;
                }

                //https://ksp.sarbian.com/jenkins/job/GCMonitor/33/artifact/GCMonitor-1.3.0.0.zip
                string downloadUrl = client.BaseUrl + "jenkins/job/" + modInfo.Name + "/" + buildNumber + "/artifact/" + artifact.fileName;

                DownloadInfo dInfo = new DownloadInfo()
                {
                    DownloadURL = downloadUrl,
                    Filename = artifact.fileName,
                    Name = buildNumber
                };
                results.Add(dInfo);
            }

            return results;
        }

        private class Project
        {
            public string name { get; set; }
            public List<BuildSummary> builds { get; set; }
        }

        private class BuildSummary
        {
            public string number { get; set; }
            public string url { get; set; }
        }

        private class Build
        {
            public List<Artifact> artifacts { get; set; }
            public string id { get; set; }
            public string result { get; set; }
            public long timestamp { get; set; }
        }

        private class Artifact
        {
            public string displayPath { get; set; }
            public string fileName { get; set; }
            public string relativePath { get; set; }
        }
    }
}
