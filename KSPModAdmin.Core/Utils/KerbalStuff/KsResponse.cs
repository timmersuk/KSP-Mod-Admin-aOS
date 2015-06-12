using System;
using Newtonsoft.Json;

namespace KSPModAdmin.Core.Utils.KerbalStuff
{
    public class KsResponse
    {
        public KsMod[] result;
        public int page = 0;
        public int pages = 0;
        public int count = 0;

        [JsonIgnore]
        public DateTime TimeStamp = DateTime.Now;
    }
}