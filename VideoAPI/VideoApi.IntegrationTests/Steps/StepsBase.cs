using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    public abstract class StepsBase
    {
        protected readonly ApiTestContext ApiTestContext;

        protected StepsBase(ApiTestContext apiTestContext)
        {
            ApiTestContext = apiTestContext;
        }
         
        protected async Task<HttpResponseMessage> SendGetRequestAsync(ApiTestContext apiTestContext)
        { 
            using (var client = apiTestContext.CreateClient())
            {                
                return await client.GetAsync(apiTestContext.Uri);
            }
        }

        protected async Task<HttpResponseMessage> SendPatchRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.CreateClient())
            {
                return await client.PatchAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPostRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.CreateClient())
            {
                return await client.PostAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPutRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.CreateClient())
            {
                return await client.PutAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendDeleteRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.CreateClient())
            {
                return await client.DeleteAsync(apiTestContext.Uri);
            }
        }
    }
}