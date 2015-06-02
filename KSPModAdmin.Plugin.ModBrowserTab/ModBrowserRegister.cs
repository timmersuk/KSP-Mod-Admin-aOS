using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPModAdmin.Plugin.ModBrowserTab
{
    public class ModBrowserRegister
    {
        #region Properties

        public Dictionary<string, IKSPMAModBrowser> ModBrowserList { get; protected set; }

        public IKSPMAModBrowser this[string modBrowserName] { get { return GetModBrowser(modBrowserName); } }

        #endregion

        public ModBrowserRegister()
        {
            ModBrowserList = new Dictionary<string, IKSPMAModBrowser>(); 
        }

        public bool Add(IKSPMAModBrowser modBrowser)
        {
            if (!ModBrowserList.ContainsKey(modBrowser.ModBrowserName))
            {
                ModBrowserList.Add(modBrowser.ModBrowserName, modBrowser);
                return true;
            }

            return false;
        }

        public IKSPMAModBrowser GetModBrowser(string name)
        {
            if (!ModBrowserList.ContainsKey(name))
                return null;

            return ModBrowserList[name];
        }

        public bool Remove(IKSPMAModBrowser modBrowser)
        {
            return RemoveByName(modBrowser.ModBrowserName);
        }

        public bool RemoveByName(string name)
        {
            var mb = GetModBrowser(name);
            if (mb != null)
            {
                ModBrowserList.Remove(mb.ModBrowserName);
                return true;
            }

            return false;
        }
    }
}
