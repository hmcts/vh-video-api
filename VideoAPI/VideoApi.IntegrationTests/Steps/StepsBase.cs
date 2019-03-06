using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    public abstract class StepsBase
    {
        protected async Task<HttpResponseMessage> SendGetRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.GetAsync(apiTestContext.Uri);
            }
        }

        protected async Task<HttpResponseMessage> SendPatchRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PatchAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPostRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PostAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendPutRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.PutAsync(apiTestContext.Uri, apiTestContext.HttpContent);
            }
        }

        protected async Task<HttpResponseMessage> SendDeleteRequestAsync(ApiTestContext apiTestContext)
        {
            using (var client = apiTestContext.Server.CreateClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiTestContext.BearerToken}");
                return await client.DeleteAsync(apiTestContext.Uri);
            }
        }
    }
}