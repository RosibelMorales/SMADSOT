using Microsoft.Extensions.Configuration;
using Smadot.Utilities.CustomAttributes.Filters.Interfaces;
using static Smadot.Utilities.CustomAttributes.Filters.Classes.ApiKeyValidator;

namespace Smadot.Utilities.CustomAttributes.Filters.Classes
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly string apiKey;
        public ApiKeyValidator(IConfiguration configuration)
        {
            apiKey = configuration["SmadotAPISettings:APIKey"] ?? "8e8a3e9c-a448-4542-a715-aa1f69cb455c";
        }
        public bool IsValid(string apiKey = "")
        {
            apiKey ??= string.Empty;
            if (apiKey.Equals(this.apiKey ?? ""))
                return true;
            return false;
        }
    }
}
