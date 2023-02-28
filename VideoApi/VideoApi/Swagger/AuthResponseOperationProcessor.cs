using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace VideoApi.Swagger
{
    [ExcludeFromCodeCoverage]
    public class AuthResponseOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (!(context is AspNetCoreOperationProcessorContext)) return true;
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext) context;

            var endpointMetadata =
                aspNetCoreContext.ApiDescription?.ActionDescriptor?.TryGetPropertyValue<IList<object>>(
                    "EndpointMetadata");
            if (endpointMetadata == null) return true;
            var allowAnonymous = endpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return true;
            }

            var authorizeAttributes = endpointMetadata.OfType<AuthorizeAttribute>().ToList();
            if (!authorizeAttributes.Any())
            {
                return true;
            }

            aspNetCoreContext.OperationDescription.Operation.Responses.Add("401", new NSwag.OpenApiResponse()
            {
                Description = "Unauthorized"
            });

            return true;
        }
    }
}
