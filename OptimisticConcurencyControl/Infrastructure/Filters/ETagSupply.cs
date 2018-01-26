using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using OptimisticConcurencyControl.Infrastructure.Attributes;

namespace OptimisticConcurencyControl.Infrastructure.Filters
{
    public class ETagSupply : ResultFilterAttribute
    {        
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult result)
            {                
                var rowVersion = result.Value?.GetType().GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttributes().Any(a => a.GetType() == typeof(RowVersionAttribute)));
                var rowVersionValue = rowVersion?.GetValue(result.Value);
                if (rowVersionValue is byte[])
                {
                    context.HttpContext.Response.Headers[HeaderNames.ETag] = 
                        $"\"{Convert.ToBase64String(rowVersionValue as byte[], Base64FormattingOptions.None)}\"";                    
                }
            }
        }
    }
}
