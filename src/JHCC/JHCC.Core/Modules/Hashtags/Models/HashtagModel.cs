﻿using System;

namespace JHCC.Core.Modules.Hashtags.Models
{
    public class HashtagModel
    {
        public Guid UniqueId { get; set; }

        public string Tag { get; set; }

        public int Count { get; set; }
    }
}
