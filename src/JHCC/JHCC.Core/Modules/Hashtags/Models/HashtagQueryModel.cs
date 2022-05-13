using System;
using System.Collections.Generic;

namespace JHCC.Core.Modules.Hashtags.Models
{
    public class HashtagQueryModel
    {        
        public List<Guid> UniqueIds { get; set; }

        public List<string> Tags { get; set; }

        public string SearchString { get; set; }

        public HashtagQueryModel()
        {
            UniqueIds = new List<Guid>();
            Tags = new List<string>();
        }
    }
}
