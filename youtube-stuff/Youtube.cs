using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Hypster2Youtube.Properties;

namespace Hypster2Youtube
{
    public class Youtube
    {
        private YouTubeService yt;

        public async Task Login()
        {
            UserCredential credential;
            using (var stream = GenerateStreamFromString(Resources.client_secrets_json))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(Program.AppName)
                );
            }

            yt = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Program.AppName
            });
        }

        public async Task InsertPlaylist(HypsterPlaylist playlist, string description = "", bool makePrivate = true)
        {
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = playlist.Name;
            newPlaylist.Snippet.Description = description;
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = makePrivate ? "private" : "public";
            newPlaylist = await yt.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            foreach (var songId in playlist.SongIds)
            {
                var video = new PlaylistItem();
                video.Snippet = new PlaylistItemSnippet();
                video.Snippet.PlaylistId = newPlaylist.Id;
                video.Snippet.ResourceId = new ResourceId();
                video.Snippet.ResourceId.Kind = "youtube#video";
                video.Snippet.ResourceId.VideoId = songId;
                video = await yt.PlaylistItems.Insert(video, "snippet").ExecuteAsync();
            }
        }

        private static MemoryStream GenerateStreamFromString(string value)
            => new MemoryStream(Encoding.UTF8.GetBytes(value));
    }
}
