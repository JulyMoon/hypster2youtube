using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
        }

        private static List<string> GetSongIds(string contents)
        {
            const string before = "items_arr[";

            var result = new List<string>();

            int currentIndex = 0;
            while (true)
            {
                currentIndex = contents.IndexOf(before, currentIndex);
                if (currentIndex == -1)
                    break;

                currentIndex = contents.IndexOf('\'', currentIndex) + 1;
                result.Add(contents.Substring(currentIndex, 11));
            }

            return result;
        }

        private static string GetName(string contents)
        {
            const string before = "0px; overflow:hidden;\">";
            int beforeIndex = contents.IndexOf(before);
            int startIndex = beforeIndex + before.Length;
            int endIndex = contents.IndexOf('<', startIndex);

            if (beforeIndex == -1 || endIndex == -1)
                throw new ArgumentException("This is not a hypster playlist or this parser is outdated");

            return WebUtility.HtmlDecode(contents.Substring(startIndex, endIndex - startIndex).Trim());
        }

        private static string GetWebContents(string link)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(link);
            }
        }
    }
}
