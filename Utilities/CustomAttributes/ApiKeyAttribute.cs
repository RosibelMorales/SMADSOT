using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.CustomAttributes.Filters;

namespace Smadot.Utilities.CustomAttributes
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
        : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
