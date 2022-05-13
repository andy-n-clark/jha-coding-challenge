using JHCC.Core.Infrastructure.MassTransit.Contracts;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JHCC.Core.Modules.Tweets.Commands
{
    public class StartSampleStreamRequest : IRequest<StartSampleStreamResponse>
    {
        public int MaxHashtagsToTrack { get; set; }
    }

    public class StartSampleStreamResponse
    {
        public bool WasStarted { get; set; }

        public StartSampleStreamResponse()
        {
            WasStarted = false;
        }
    }

    public class StartSampleStreamRequestHandler : IRequestHandler<StartSampleStreamRequest, StartSampleStreamResponse>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public StartSampleStreamRequestHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task<StartSampleStreamResponse> Handle(StartSampleStreamRequest request, CancellationToken token)
        {
            var response = new StartSampleStreamResponse();

            await _publishEndpoint.Publish<IStartSampleStream>(new
            {
                MaxHashtagsToTrack = request.MaxHashtagsToTrack
            });

            response.WasStarted = true;

            return response;
        }
    }
}
