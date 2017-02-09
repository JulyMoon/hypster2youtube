using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hypster2Youtube
{
    public class HypsterPlaylist
    {
        public string Name { get; private set; }
        public List<string> SongIds;

        public HypsterPlaylist(string playlistLink)
        {
            var contents = GetWebContents(playlistLink);
            Name = GetName(contents);
            SongIds = GetSongIds(contents);
            //System.Net.WebUtility.HtmlDecode()
        }

        private static List<string> GetSongIds(string contents)
        {
            throw new NotImplementedException();
        }

        private static string GetName(string contents)
        {
            throw new NotImplementedException();
        }

        private static string GetWebContents(string link)
        {
            using (WebClient client = new WebClient())
                return client.DownloadString(link);
        }
    }
}
