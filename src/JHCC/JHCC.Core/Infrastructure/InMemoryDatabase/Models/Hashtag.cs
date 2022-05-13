using System;
using System.ComponentModel.DataAnnotations;

namespace JHCC.Core.Infrastructure.InMemoryDatabase.Models
{
    public class Hashtag
    {
        [Key]
        public Guid UniqueId { get; set; }

        public string Tag { get; set; }

        public int Count { get; set; }
    }
}
