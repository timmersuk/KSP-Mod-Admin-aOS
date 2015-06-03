using System;
using System.Diagnostics.CodeAnalysis;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CkanResource
    {
        public Uri homepage;
        public Uri bugtracker;
        public Uri license;
        public Uri ci;
        public Uri kerbalstuff;
        public Uri manual;
    }
}