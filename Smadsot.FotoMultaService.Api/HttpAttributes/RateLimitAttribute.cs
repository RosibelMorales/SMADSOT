using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Smadsot.FotoMultaService.Api.HttpAttributes.Configuration;

namespace Smadsot.FotoMultaService.Api.HttpAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : TypeFilterAttribute
    {
        public RateLimitAttribute() : base(typeof(RateLimitFilter))
        {
        }
    }

}