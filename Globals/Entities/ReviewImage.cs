﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class ReviewImage
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual Review Review { get; set; }
    }
}