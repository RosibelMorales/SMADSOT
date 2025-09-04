using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smadsot.FotoMultaService.Api.HttpAttributes.Configuration
{
    public class RateLimitOptions
    {
        public int Requests { get; set; }
        public int Seconds { get; set; }
    }

}