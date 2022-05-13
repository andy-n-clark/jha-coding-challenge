using System;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;

namespace JHCC.ConsoleTest
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("started...");

            // ------------------------------- "CONSUMER_KEY", "CONSUMER_SECRET", "BEARER_TOKEN"
            var userClient = new TwitterClient("qpfVheycwNPMxmFBw9zB64VBf", "7sg1vtNsPfI6AsoeFMdIAeP1ZdoogXxaZZuysEBrgpNyQWOSGG", "AAAAAAAAAAAAAAAAAAAAAH3ZcQEAAAAAf2kUPjowM3gFxHUx5yLclD%2F0XKc%3DwamY9k1KLoz2SSS3Yr5NeGAlDunvF2gnM0cQQth7c7S8GrVgaA");

            Console.WriteLine("created userClient...");

            var sampleStream = userClient.StreamsV2.CreateSampleStream();

            Console.WriteLine("created sampleStream...");

            sampleStream.TweetReceived += (sender, eventArgs) =>
            {
                if (eventArgs.Tweet.Entities.Hashtags != null && eventArgs.Tweet.Entities.Hashtags.Length > 0)
                {
                    Console.WriteLine(string.Join(", ", eventArgs.Tweet.Entities.Hashtags.ToList().Select(i => i.Tag)));
                    //Console.WriteLine(eventArgs.Tweet.Entities.Hashtags);
                }
            };

            //sampleStream.EventReceived += SampleStream_EventReceived;

            await sampleStream.StartAsync();

            Console.WriteLine("started sampleStream...");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 0;
        }

        private static void SampleStream_EventReceived(object sender, Tweetinvi.Events.StreamEventReceivedArgs e)
        {
            Console.WriteLine(e.Json.ToString());
        }
    }
}
