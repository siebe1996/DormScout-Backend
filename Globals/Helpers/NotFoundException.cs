﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Helpers
{
    public class NotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public NotFoundException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.NotFound;
        }
    }
}
