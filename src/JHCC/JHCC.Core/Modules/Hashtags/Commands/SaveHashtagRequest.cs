using JHCC.Core.Modules.Hashtags.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JHCC.Core.Modules.Hashtags.Commands
{
    public class SaveHashtagRequest : IRequest<SaveHashtagResponse>
    {
        public HashtagModel Hashtag { get; set; }
    }

    public class SaveHashtagResponse
    {
        public HashtagModel Hashtag { get; set; }

        public bool WasSaved { get; set; }

        public SaveHashtagResponse()
        {
            Hashtag = null;

            WasSaved = false;
        }
    }

    public class SaveHashtagRequestHandler : IRequestHandler<SaveHashtagRequest, SaveHashtagResponse>
    {
        private readonly IHashtagService _hashtagService;

        public SaveHashtagRequestHandler(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }

        public async Task<SaveHashtagResponse> Handle(SaveHashtagRequest request, CancellationToken token)
        {
            // Validate request
            //var validator = new SaveHashtagRequestValidator();
            //validator.ValidateAndThrow(request);

            var response = new SaveHashtagResponse();

            var results = await _hashtagService.FindHashtags(new HashtagQueryModel() { Tags = new List<string>() { request.Hashtag.Tag } });

            if (results != null && results.Count > 0)
            {
                var result = results.FirstOrDefault(q => q.Tag == request.Hashtag.Tag);
                
                if (result != null && result.Tag == request.Hashtag.Tag)
                {
                    // TODO: add Automapper mappers...
                    //var model = _mapper.Map<HashtagEditModel>(request.Hashtag);
                    var model = new HashtagEditModel();
                    model.UniqueId = result.UniqueId;
                    model.Tag = result.Tag;
                    model.Count = result.Count + 1;

                    result = await _hashtagService.EditHashtag(model);

                    response.Hashtag = result;
                    response.WasSaved = true;
                }
                else
                {
                    // TODO: add Automapper mappers...
                    //var model = _mapper.Map<HashtagCreateModel>(request.Hashtag);
                    var model = new HashtagCreateModel();
                    model.Tag = result.Tag;

                    result = await _hashtagService.CreateHashtag(model);

                    response.Hashtag = result;
                    response.WasSaved = true;
                }
            }
            else
            {
                // TODO: add Automapper mappers...
                //var model = _mapper.Map<HashtagCreateModel>(request.Hashtag);
                var model = new HashtagCreateModel();
                model.Tag = request.Hashtag.Tag;

                var result = await _hashtagService.CreateHashtag(model);

                response.Hashtag = result;
                response.WasSaved = true;
            }

            return response;
        }
    }
}
