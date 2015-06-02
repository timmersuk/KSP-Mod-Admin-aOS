using System.Collections.Generic;
using Newtonsoft.Json;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanInstallInfo
    {
        public string file;
        public string find;
        public string install_to;
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> filter;
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> filter_regexp;
    }
}