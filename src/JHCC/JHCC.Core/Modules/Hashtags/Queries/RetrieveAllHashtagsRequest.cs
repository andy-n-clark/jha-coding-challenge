using JHCC.Core.Modules.Hashtags.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JHCC.Core.Modules.Hashtags.Queries
{
    public class RetrieveAllHashtagsRequest : IRequest<RetrieveAllHashtagsResponse>
    {
    }

    public class RetrieveAllHashtagsResponse
    {
        public List<HashtagModel> Hashtags { get; set; }

        public RetrieveAllHashtagsResponse()
        {
            Hashtags = new List<HashtagModel>();
        }
    }

    public class RetrieveAllHashtagsRequestHandler : IRequestHandler<RetrieveAllHashtagsRequest, RetrieveAllHashtagsResponse>
    {
        private readonly IHashtagService _hashtagService;

        public RetrieveAllHashtagsRequestHandler(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }

        public async Task<RetrieveAllHashtagsResponse> Handle(RetrieveAllHashtagsRequest request, CancellationToken token)
        {
            // Validate request
            //var validator = new RetrieveAllHashtagsRequestValidator();
            //validator.ValidateAndThrow(request);

            var response = new RetrieveAllHashtagsResponse();

            response.Hashtags = await _hashtagService.FindHashtags(new HashtagQueryModel());

            return response;
        }
    }
}
