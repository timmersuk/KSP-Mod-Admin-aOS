using KSPMODAdmin.Core.Utils.Ckan;
using KSPModAdmin.Core.Utils.Controls.Aga.Controls.Tree;

namespace KSPModAdmin.Plugin.ModBrowserTab.Model
{
    public class CkanTreeModel : TreeModel
    {
        public void AddArchive(CkanArchive archive)
        {
            foreach (var mod in archive.Mods)
                Nodes.Add(new CkanNode(mod.Value));
        }
    }
}