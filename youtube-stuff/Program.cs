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
    class Program
    {
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private const string playlistLink = @"http://hypster.com/playlists/userid/5049121";

        [STAThread]
        static void Main(string[] args)
        {
            foreach (var song in new HypsterPlaylist(playlistLink).SongIds)
                Console.WriteLine(song);

            /*Console.WriteLine("Hypster2Youtube");
            Console.WriteLine("==================================");

            try
            {
                Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }*/

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task Run()
        {
            UserCredential credential;
            using (var stream = GenerateStreamFromString(Resources.client_secrets_json))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(AppName)
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName
            });

            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = "Test Playlist";
            newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "private";
            newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
        }

        public static MemoryStream GenerateStreamFromString(string value)
            => new MemoryStream(Encoding.UTF8.GetBytes(value));
    }
}