using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace OptimisticConcurencyControl.Middlewares.ETag
{
    public class ETagMiddleware
    {
        private readonly RequestDelegate _next;

        public ETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;
            using (var ms = new MemoryStream())
            {
                response.Body = ms;
                await _next(context);

                if (IsEtagSupported(response))
                {
                    var checksum = CalculateChecksum(ms);
                    response.Headers[HeaderNames.ETag] = checksum;

                    if (context.Request.Headers.TryGetValue(HeaderNames.IfMatch, out var etag) && 
                        checksum == etag)
                    {
                        
                    }
                }
            }            
        }

        private static bool IsEtagSupported(HttpResponse response)
        {
            if (response.StatusCode != StatusCodes.Status200OK)
                return false;

            // The 20kb length limit is not based in science. Feel free to change
            if (response.Body.Length > 20 * 1024)
                return false;

            if (response.Headers.ContainsKey(HeaderNames.ETag))
                return false;

            return true;
        }

        private static string CalculateChecksum(MemoryStream ms)
        {            
            using (var algo = SHA1.Create())
            {
                ms.Position = 0;
                byte[] bytes = algo.ComputeHash(ms);
                return $"\"{WebEncoders.Base64UrlEncode(bytes)}\"";
            }            
        }
    }
}
