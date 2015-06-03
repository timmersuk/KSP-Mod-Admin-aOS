using System;
using System.Diagnostics.CodeAnalysis;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    /// <summary>
    /// Class that contains the CKAN Repository infos.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."),
     SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CkanRepository
    {
        public string name = string.Empty;
        public Uri uri;

        public override string ToString()
        {
            return String.Format("{0} ({1})", name, uri);
        }
    }

    /// <summary>
    /// Class that contains all available CKAN Repositories.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."),
     SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public struct CkanRepositories
    {
        public CkanRepository[] repositories;
    }
}
