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
    // [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitFilter : IAsyncActionFilter
    {
        private readonly int _requests;
        private readonly int _seconds;

        public RateLimitFilter(IOptions<RateLimitOptions> options)
        {
            _requests = options.Value.Requests;
            _seconds = options.Value.Seconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = ObtainSessionTokenFromRequest(context); // Implementa la lógica para obtener el token de sesión desde la solicitud

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new BadRequestResult(); // Tratar la falta de token como una solicitud incorrecta
                return;
            }

            var cacheKey = $"{token}_Requests";
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            var cacheEntry = cache.TryGetValue<int>(cacheKey, out var currentRequests)
                ? currentRequests
                : 0;

            if (cacheEntry >= _requests)
            {
                context.Result = new ObjectResult("Se ha alcanzado el límite de solicitudes por minuto")
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests
                };
                return;
            }

            // Incrementa el contador y actualiza la memoria caché
            cacheEntry++;
            cache.Set(cacheKey, cacheEntry, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_seconds)
            });

            await next();
        }

        private string ObtainSessionTokenFromRequest(ActionExecutingContext context)
        {
            // Implementa la lógica para obtener el token de sesión desde la solicitud,
            // puede ser un encabezado, un parámetro en la URL, o cualquier otra fuente.
            // Aquí se asume que el token está en el encabezado "Authorization".
            return context?.HttpContext?.Request?.Headers["Authorization"] ?? "";
        }
    }
}