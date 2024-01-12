﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tests
{
    public class GetHelloModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Hello
        {
            get; set;
        }
    }
}
