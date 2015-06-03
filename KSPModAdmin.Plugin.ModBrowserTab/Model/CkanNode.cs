using System.Drawing;
using System.Linq;
using KSPMODAdmin.Core.Utils.Ckan;
using KSPModAdmin.Core.Utils.Controls.Aga.Controls.Tree;
using KSPModAdmin.Plugin.ModBrowserTab.Properties;

namespace KSPModAdmin.Plugin.ModBrowserTab.Model
{
    public enum CkanNodeType
    {
        Unknown,
        Mod,
        ModInfo
    }

    public class CkanNode : Node
    {
        private CkanMod mod;
        private CkanModInfo modInfo;
        private CkanNodeType type;

        public string Name { get { return GetName(); } }
        public string Version { get { return GetVersion(); } }
        public string Author { get { return GetAuthor(); } }
        public string Description { get { return GetDescription(); } }
        public bool Checked { get; set; }
        public bool Added { get; set; }

        public bool ChildAdded
        {
            get { return Nodes.Cast<CkanNode>().Any(c => c.Added); }
        }

        public Image Icon
        {
            get
            {
                if (Added)
                    return (modInfo != null) ? Resources.component_add : Resources.folder_add;
                else
                    return (modInfo != null) ? Resources.component : Resources.folder ;
            }
        }

        public CkanNode()
        {
            type = CkanNodeType.Unknown;
        }

        public CkanNode(CkanMod mod)
        {
            this.mod = mod;
            this.type = CkanNodeType.Mod;

            foreach (var version in mod.ModInfos)
                Nodes.Add(new CkanNode(version));
        }

        public CkanNode(CkanModInfo version)
        {
            this.modInfo = version;
            this.type = CkanNodeType.ModInfo;
        }

        public string GetName()
        {
            switch (type)
            {
                case CkanNodeType.Mod:
                    return mod != null ? mod.Name : string.Empty;
                case CkanNodeType.ModInfo:
                    return modInfo != null ? modInfo.name : string.Empty;
            }

            return string.Empty;
        }

        public string GetVersion()
        {
            switch (type)
            {
                case CkanNodeType.Mod:
                    return string.Empty;
                case CkanNodeType.ModInfo:
                    return modInfo != null ? modInfo.version : string.Empty;
            }

            return string.Empty;
        }

        public string GetAuthor()
        {
            switch (type)
            {
                case CkanNodeType.Mod:
                    return string.Empty;
                case CkanNodeType.ModInfo:
                    return this.modInfo == null ? string.Empty : string.Join(", ", this.modInfo.author);
            }

            return string.Empty;
        }

        public string GetDescription()
        {
            switch (type)
            {
                case CkanNodeType.Mod:
                    return string.Empty;
                case CkanNodeType.ModInfo:
                    return modInfo != null ? modInfo.@abstract : string.Empty;
            }

            return string.Empty;
        }
    }
}
