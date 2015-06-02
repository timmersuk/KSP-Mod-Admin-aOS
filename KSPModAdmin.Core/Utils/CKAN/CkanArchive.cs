using System.Collections.Generic;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanArchive
    {
        public CkanRepository Repository { get; set; }

        public string FullPath { get; set; }

        public Dictionary<string, CkanMod> Mods { get; set; }

        public CkanArchive()
        {
            Mods = new Dictionary<string, CkanMod>();
        }
    }
}