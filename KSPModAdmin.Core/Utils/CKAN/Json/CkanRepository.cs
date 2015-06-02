using System;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanRepository
    {
        public string name;
        public Uri uri;

        public override string ToString()
        {
            return String.Format("{0} ({1})", name, uri);
        }
    }

    public struct CkanRepositories
    {
        public CkanRepository[] repositories;
    }
}
