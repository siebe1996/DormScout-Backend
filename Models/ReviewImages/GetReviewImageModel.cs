﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ReviewImages
{
    public class GetReviewImageModel : BaseReviewImageModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
