using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CkanInstallInfo
    {
        public string file = string.Empty;
        public string find = string.Empty;
        public string install_to = string.Empty;
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> filter = new List<string>();
        [JsonConverter(typeof(JsonSingleOrArrayConverter<string>))]
        public List<string> filter_regexp = new List<string>();
    }
}