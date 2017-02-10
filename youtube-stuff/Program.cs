using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hypster2Youtube
{
    class Program
    {
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private const string playlistLink = @"http://hypster.com/playlists/userid/5049121";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing hypster...");
            var playlist = new HypsterPlaylist(playlistLink);
            var youtube = new Youtube();

            Task.Run(async () =>
            {
                Console.WriteLine("Signing into youtube...");
                await youtube.Login();
                Console.WriteLine("Uploading the playlist...");
                await youtube.InsertPlaylist(playlist, "hey");
            }).GetAwaiter().GetResult();

            Console.WriteLine("Done. Press any key to continue...");
            Console.ReadKey();
        }
    }
}