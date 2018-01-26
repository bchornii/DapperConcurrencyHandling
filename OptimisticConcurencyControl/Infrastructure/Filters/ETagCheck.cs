using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using OptimisticConcurencyControl.Infrastructure.Attributes;

namespace OptimisticConcurencyControl.Infrastructure.Filters
{
    public class ETagCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(HeaderNames.IfMatch, out var etag))
            {
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var parameter = descriptor?.MethodInfo.GetParameters()
                    .FirstOrDefault(p => p.GetCustomAttributes().Any(a => a.GetType() == typeof(ETagModelAttribute)));

                if (context.ActionArguments.TryGetValue(parameter?.Name, out var model))
                {
                    var rowVersion = model?.GetType().GetProperties()
                        .FirstOrDefault(p => p.GetCustomAttributes().Any(a => a.GetType() == typeof(ConcurrencyVersionAttribute)));
                    if (rowVersion?.CanWrite ?? false)
                    {
                        var encodedETag = etag.ToString().Trim("\"".ToCharArray());
                        rowVersion.SetValue(model, Convert.FromBase64String(encodedETag));
                    }
                }
            }
        }
    }
}