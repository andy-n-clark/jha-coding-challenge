using JHCC.Core.Infrastructure.InMemoryDatabase;
using JHCC.Core.Infrastructure.InMemoryDatabase.Models;
using JHCC.Core.Modules.Hashtags.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JHCC.Core.Modules.Hashtags
{
    public interface IHashtagService
    {
        Task<HashtagModel> CreateHashtag(HashtagCreateModel createModel);

        Task<HashtagModel> EditHashtag(HashtagEditModel editModel);

        Task<List<HashtagModel>> FindHashtags(HashtagQueryModel query);
    }

    public class HashtagService : IHashtagService
    {
        private readonly DatabaseContext _context;

        private readonly ILogger<HashtagService> _logger;

        public HashtagService(DatabaseContext context, ILogger<HashtagService> logger)
        {
            _context = context;

            _logger = logger;
        }

        public async Task<HashtagModel> CreateHashtag(HashtagCreateModel createModel)
        {
            // TODO: add Automapper mappers...
            //var hashtagModel = _mapper.Map<HashtagModel>(createModel);
            var hashtagModel = new HashtagModel();
            hashtagModel.UniqueId = Guid.NewGuid();
            hashtagModel.Tag = createModel.Tag;
            hashtagModel.Count = 1;

            // Validate the request
            //_hashtagModelValidator.ValidateAndThrow(hashtagModel);

            // TODO: add Automapper mappers...
            //var hashtag = _mapper.Map<Hashtag>(hashtagModel);            
            var hashtag = new Hashtag();
            hashtag.UniqueId = hashtagModel.UniqueId;
            hashtag.Tag = hashtagModel.Tag;
            hashtag.Count = hashtagModel.Count;

            // Save changes...
            await _context.Hashtags.AddAsync(hashtag);
            await _context.SaveChangesAsync();

            //hashtagModel.UniqueId = hashtag.UniqueId;

            // Return newly created hashtag...
            var hashtags = await FindHashtags(new HashtagQueryModel { Tags = new List<string> { hashtag.Tag } }); // UniqueIds = new List<Guid> { hashtag.UniqueId } });
            return hashtags.FirstOrDefault();
        }

        public async Task<HashtagModel> EditHashtag(HashtagEditModel editModel)
        {
            // Load existing data and map new data over existing
            var hashtag = await _context.Hashtags
                .FirstOrDefaultAsync(hashtag => hashtag.UniqueId == editModel.UniqueId);

            // TODO: add Automapper mappers...
            //_mapper.Map(editModel, hashtag);
            hashtag.Tag = editModel.Tag;
            hashtag.Count = editModel.Count;

            // TODO: add Automapper mappers...
            //var hashtagModel = _mapper.Map<HashtagModel>(hashtag);
            var hashtagModel = new HashtagModel();
            hashtagModel.UniqueId = hashtag.UniqueId;
            hashtagModel.Tag = hashtag.Tag;
            hashtagModel.Count = hashtag.Count;

            // Validate the request
            //_hashtagModelValidator.ValidateAndThrow(hashtagModel);

            // Save changes...
            await _context.SaveChangesAsync();

            // Return hashtag...
            var hashtags = await FindHashtags(new HashtagQueryModel { UniqueIds = new List<Guid> { hashtag.UniqueId } });
            return hashtags.FirstOrDefault();
        }

        public async Task<List<HashtagModel>> FindHashtags(HashtagQueryModel query)
        {
            // Create queryable
            IQueryable<Hashtag> hashtagQuery = _context.Hashtags;

            if (query.UniqueIds != null && query.UniqueIds.Count > 0)
            {
                hashtagQuery = hashtagQuery.Where(hashtag => query.UniqueIds.Contains(hashtag.UniqueId));
            }

            if (query.Tags != null && query.Tags.Count > 0)
            {
                hashtagQuery = hashtagQuery.Where(hashtag => query.Tags.Contains(hashtag.Tag));
            }            

            if (query.SearchString != null && query.SearchString.Length > 0)
            {
                hashtagQuery = hashtagQuery.Where(hashtag => hashtag.Tag.ToLower().Contains(query.SearchString.ToLower()));
            }

            // Return results
            var hashtags = await hashtagQuery.OrderByDescending(q => q.Count).ToListAsync();

            // TODO: add Automapper mappers...
            //return _mapper.Map<List<HashtagModel>>(hashtags);

            var results = new List<HashtagModel>();

            foreach (var hashtag in hashtags)
            {
                var model = new HashtagModel();

                model.UniqueId = hashtag.UniqueId;
                model.Tag = hashtag.Tag;
                model.Count = hashtag.Count;

                results.Add(model);
            }

            return results;
        }
    }
}
