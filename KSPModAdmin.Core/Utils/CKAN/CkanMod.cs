using System.Collections.Generic;

namespace KSPMODAdmin.Core.Utils.Ckan
{
    public class CkanMod
    {
        public string Name { get; set; }

        public List<CkanModInfo> ModInfos { get; set; }

        public string ArchivePath { get; set; }

        public CkanMod()
        {
            ModInfos = new List<CkanModInfo>();
        }
    }
}