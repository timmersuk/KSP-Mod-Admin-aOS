using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanModInfo
    {
        [JsonIgnore]
        public CkanMod Mod { get; set; }

        public string spec_version;
        public string name;
        public string @abstract;
        public string identifier;
        public Uri download;
        public string license;
        public string version;
        public uint epoch;
        public string mod_version;

        public CkanInstallInfo[] install;

        public string comment;
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> author;
        public string description;
        public string release_status;
        public string ksp_version;
        public string ksp_version_min;
        public string ksp_version_max;

        public CkanResource resources;
        public CkanRelation[] depands;
        public CkanRelation[] recommends;
        public CkanRelation[] suggests;
        public CkanRelation[] conflicts;

        public string download_size;
    }
}