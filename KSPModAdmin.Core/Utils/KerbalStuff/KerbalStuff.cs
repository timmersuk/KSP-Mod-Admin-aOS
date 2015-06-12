using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KSPModAdmin.Core.Utils.KerbalStuff
{
    public static class KerbalStuff
    {
        private static readonly string KerbalStuffUrl = "https://kerbalstuff.com";
        private static readonly string KerbalStuffApiUrl = KerbalStuffUrl + "/api";
        private static readonly string KerbalStuffBrowse = KerbalStuffApiUrl + "/browse?page={0}";
        private static readonly string KerbalStuffBrowseCount = KerbalStuffBrowse + "&count={1}";
        private static readonly string KerbalStuffBrowseNew = KerbalStuffApiUrl + "/browse/new?page={0}";
        private static readonly string KerbalStuffBrowseFeatured = KerbalStuffApiUrl + "/browse/featured?page={0}";
        private static readonly string KerbalStuffBrowseTop = KerbalStuffApiUrl + "/browse/top?page={0}";
        private static readonly string KerbalStuffModInfo = KerbalStuffApiUrl + "/browse/mod/{0}";

        private static Dictionary<string, KsResponse> cache = new Dictionary<string, KsResponse>();

        public static KsResponse LastResponse { get; private set; }

        public static Exception Error { get; private set; }

        public static bool UseCaching { get; set; }


        static KerbalStuff()
        {
            UseCaching = true;
        }


        public static void ClearCache()
        {
            cache.Clear();
        }

        public static List<KsMod> Browse(int page, int count = -1)
        {
            var url = count <= -1 ? string.Format(KerbalStuffBrowse, page) : string.Format(KerbalStuffBrowseCount, page, count);
            return GetModsFromKsResponse(url);
        }

        public static List<KsMod> BrowseNew(int page)
        {
            var url = string.Format(KerbalStuffBrowseNew, page);
            return GetMods(url);
        }

        public static List<KsMod> BrowseFeatured(int page)
        {
            var url = string.Format(KerbalStuffBrowseFeatured, page);
            return GetMods(url);
        }

        public static List<KsMod> BrowseTop(int page)
        {
            var url = string.Format(KerbalStuffBrowseTop, page);
            return GetMods(url);
        }

        public static List<KsMod> BrowseMod(string page)
        {
            var url = string.Format(KerbalStuffModInfo, page);
            return GetMods(url);
        }


        private static List<KsMod> GetMods(string url)
        {
            try
            {
                Error = null;

                // try get data from cache if present and not older then a minute.
                if (UseCaching && cache.ContainsKey(url) && cache[url].TimeStamp > DateTime.Now.AddMinutes(1))
                    LastResponse = cache[url];
                else
                {
                    var content = Www.Load(url);
                    var mods = JsonConvert.DeserializeObject<List<KsMod>>(content);
                    LastResponse = new KsResponse()
                        {
                            page = 1,
                            pages = 1000,
                            count = mods.Count,
                            result = mods.ToArray()
                        };

                    if (UseCaching)
                    {
                        if (!cache.ContainsKey(url))
                            cache.Add(url, LastResponse);
                        else
                            cache[url] = LastResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex;
                Messenger.AddError(string.Format("Error during get KerbelStuff mods from \"{0}\"! {1}", url, ex.Message), ex);
            }

            if (LastResponse != null)
                return LastResponse.result.ToList();

            return new List<KsMod>();
        }

        private static List<KsMod> GetModsFromKsResponse(string url)
        {
            try
            {
                Error = null;
                LastResponse = GetKsResponse(url);
            }
            catch (Exception ex)
            {
                Error = ex;
                Messenger.AddError(string.Format("Error during get KerbelStuff mods from \"{0}\"! {1}", url, ex.Message), ex);
            }

            if (LastResponse != null)
                return LastResponse.result.ToList();

            return new List<KsMod>();
        }

        private static KsResponse GetKsResponse(string url)
        {
            KsResponse response = null;

            // try get data from cache if present and not older then a minute.
            if (UseCaching && cache.ContainsKey(url) && cache[url].TimeStamp > DateTime.Now.AddMinutes(1)) 
                response = cache[url];
            else
            {
                var content = Www.Load(url);
                response = JsonConvert.DeserializeObject<KsResponse>(content);

                if (UseCaching)
                {
                    if (!cache.ContainsKey(url)) 
                        cache.Add(url, response);
                    else
                        cache[url] = response;
                }
            }

            return response;
        }
    }
}
