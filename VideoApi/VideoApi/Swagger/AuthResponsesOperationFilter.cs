using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Video.API.Swagger
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is IAllowAnonymousFilter);
            
            if (!isAuthorized || allowAnonymous) return;
            var unauthorisedStatus = ((int)HttpStatusCode.Unauthorized).ToString();

            if (!operation.Responses.ContainsKey(unauthorisedStatus))
            {
                operation.Responses.Add(unauthorisedStatus, new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });   
            }
        }
    }
}
