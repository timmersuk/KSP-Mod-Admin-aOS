using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CkanModInfo
    {
        [JsonIgnore]
        public CkanMod Mod { get; set; }

        public string spec_version = string.Empty;
        public string name = string.Empty;
        public string @abstract = string.Empty;
        public string identifier = string.Empty;
        public Uri download;
        public string license = string.Empty;
        public string version = string.Empty;
        public uint epoch = 0;
        public string mod_version = string.Empty;

        public CkanInstallInfo[] install;

        public string comment = string.Empty;
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> author = new List<string>();
        public string description = string.Empty;
        public string release_status = string.Empty;
        public string ksp_version = string.Empty;
        public string ksp_version_min = string.Empty;
        public string ksp_version_max = string.Empty;

        public CkanResource resources;
        public CkanRelation[] depands;
        public CkanRelation[] recommends;
        public CkanRelation[] suggests;
        public CkanRelation[] conflicts;

        public string download_size = string.Empty;
    }
}