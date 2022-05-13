using JHCC.Core.Infrastructure.MassTransit.Contracts;
using JHCC.Core.Modules.Hashtags.Commands;
using JHCC.Core.Modules.Hashtags.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;

namespace JHCC.Core.Infrastructure.MassTransit.Consumers
{
    public class StartSampleStreamConsumer : IConsumer<IStartSampleStream>
    {
        private readonly ILogger<StartSampleStreamConsumer> _logger;

        private readonly ITwitterClient _twitterClient;

        private readonly IMediator _mediator;

        private readonly IMemoryCache _memoryCache;

        private readonly string sampleStreamRunningKey = "sampleStreamRunningKey";
        private readonly string tweetsCounterKey = "tweetsCounterKey";

        private readonly MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));

        public StartSampleStreamConsumer(ILogger<StartSampleStreamConsumer> logger, IConfiguration configuration, IMediator mediator, IMemoryCache memoryCache)
        {
            _logger = logger;

            // TODO: make these configuration values be provided via DI...
            _twitterClient = new TwitterClient(configuration["Authentication:Twitter:ConsumerKey"], configuration["Authentication:Twitter:ConsumerSecret"], configuration["Authentication:Twitter:BearerToken"]);

            _mediator = mediator;

            _memoryCache = memoryCache;
        }

        public async Task Consume(ConsumeContext<IStartSampleStream> context)
        {
            _logger.LogInformation($"{nameof(StartSampleStreamConsumer)}: Starting sample stream. UTC: {DateTime.UtcNow}.");

            var sampleStream = _twitterClient.StreamsV2.CreateSampleStream();

            var count = 0;

            sampleStream.TweetReceived += async (sender, eventArgs) =>
            {
                var tweetsCounter = 0;

                if (_memoryCache.TryGetValue(tweetsCounterKey, out tweetsCounter))
                {
                    _memoryCache.Set(tweetsCounterKey, tweetsCounter + 1, cacheOptions);
                }
                else
                {
                    _memoryCache.Set(tweetsCounterKey, 1, cacheOptions);
                }

                _logger.LogInformation($"{nameof(StartSampleStreamConsumer)}: Tweet was received. UTC: {DateTime.UtcNow}.");

                if (eventArgs.Tweet.Entities.Hashtags != null && eventArgs.Tweet.Entities.Hashtags.Length > 0)
                {
                    var hashtags = eventArgs.Tweet.Entities.Hashtags.ToList();

                    foreach (var hashtag in hashtags)
                    {
                        var request = new SaveHashtagRequest() { Hashtag = new HashtagModel() { Tag = hashtag.Tag } };

                        var response = await _mediator.Send(request);

                        if (response.WasSaved)
                        {
                            count++;
                        }
                    }
                }                

                if (count > context.Message.MaxHashtagsToTrack)
                {
                    sampleStream.StopStream();

                    _memoryCache.Set(sampleStreamRunningKey, false, cacheOptions);

                    _logger.LogInformation($"{nameof(StartSampleStreamConsumer)}: Stopped sample stream. UTC: {DateTime.UtcNow}.");
                }
            };

            await sampleStream.StartAsync();            
        }
    }
}

