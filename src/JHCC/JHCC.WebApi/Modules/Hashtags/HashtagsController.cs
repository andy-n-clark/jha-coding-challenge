using JHCC.Core.Modules.Hashtags.Commands;
using JHCC.Core.Modules.Hashtags.Queries;
using JHCC.Core.Modules.Tweets.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JHCC.WebApi.Modules.Hashtags
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashtagsController : ControllerBase
    {
        private readonly ILogger<HashtagsController> _logger;

        private readonly IMediator _mediator;

        private readonly IMemoryCache _memoryCache;

        private readonly string sampleStreamRunningKey = "sampleStreamRunningKey";
        private readonly string tweetsCounterKey = "tweetsCounterKey";

        private readonly MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));

        public HashtagsController(ILogger<HashtagsController> logger, IMediator mediator, IMemoryCache memoryCache)
        {
            _logger = logger;

            _mediator = mediator;

            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("GetHashtags")]
        public async Task<IActionResult> GetHashtags(CancellationToken cancellationToken)
        {
            _logger.LogInformation(@"Called GetHashtags Endpoint");

            var request = new RetrieveAllHashtagsRequest();

            var response = await _mediator.Send(request, cancellationToken);

            if (response == null || response.Hashtags == null || response.Hashtags.Count == 0)
                return NotFound("No hashtags were found...");

            return Ok(response);
        }

        [HttpPost]
        [Route("SaveHashtag")]
        public async Task<IActionResult> SaveHashtag([FromBody] SaveHashtagRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Route("StartSampleStream/{maxHashtagsToTrack}")]
        public async Task<IActionResult> StartSampleStream(int maxHashtagsToTrack, CancellationToken cancellationToken)
        {
            var isSampleStreamRunning = false;

            if (_memoryCache.TryGetValue(sampleStreamRunningKey, out isSampleStreamRunning))
            {
                if (!isSampleStreamRunning)
                {
                    var request = new StartSampleStreamRequest() { MaxHashtagsToTrack = maxHashtagsToTrack };
                    var response = await _mediator.Send(request, cancellationToken);

                    if (response.WasStarted)
                    {
                        _memoryCache.Set(sampleStreamRunningKey, true, cacheOptions);

                        return Ok("Sample stream was started successfully.");
                    }
                    else
                    {
                        return Ok("Sample stream was not started successfully.");
                    }
                }
                else
                {
                    return Ok("Sample stream was already running.");
                }
            }
            else
            {
                var request = new StartSampleStreamRequest() { MaxHashtagsToTrack = maxHashtagsToTrack };
                var response = await _mediator.Send(request, cancellationToken);

                if (response.WasStarted)
                {
                    _memoryCache.Set(sampleStreamRunningKey, true, cacheOptions);

                    return Ok("Sample stream was started successfully.");
                }
                else
                {
                    return Ok("Sample stream was not started successfully.");
                }
            }
        }

        [HttpGet]
        [Route("GetTweetsCounter")]
        public async Task<IActionResult> GetTweetsCounter(CancellationToken cancellationToken)
        {
            _logger.LogInformation(@"Called GetTweetsCounter Endpoint");

            var tweetsCounter = 0;

            if (_memoryCache.TryGetValue(tweetsCounterKey, out tweetsCounter))
            {
                return Ok(tweetsCounter);
            }

            return Ok(tweetsCounter);
        }
    }
}

