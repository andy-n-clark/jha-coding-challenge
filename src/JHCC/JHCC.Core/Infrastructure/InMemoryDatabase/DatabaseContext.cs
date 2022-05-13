using JHCC.Core.Infrastructure.InMemoryDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Tweetinvi.Core.DTO;
using Tweetinvi.Core.Models;

namespace JHCC.Core.Infrastructure.InMemoryDatabase
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        //public DbSet<TweetDTO> Tweets { get; set; }

        public DbSet<Hashtag> Hashtags { get; set; }
    }
}
