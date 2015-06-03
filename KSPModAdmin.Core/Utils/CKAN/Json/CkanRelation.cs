using System.Diagnostics.CodeAnalysis;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CkanRelation
    {
        public string name = string.Empty;
        public string version = string.Empty;
        public string min_version = string.Empty;
        public string max_version = string.Empty;
    }
}