using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Extensions;
using Newtonsoft.Json;

namespace MewPipe.ApiClient
{
    public partial class MewPipeApiClient
    {
        public async Task<ImpressionContract> GetVideoImpression(string userId, string publicVideoId)
        {
            return await _httpClient.SendGet<ImpressionContract>(String.Format("impressions/{0}/{1}", userId, publicVideoId));
        }

        public async Task<VideoContract> SetVideoImpression(ImpressionContract contract)
        {
            var content = new StringContent(JsonConvert.SerializeObject(contract), Encoding.UTF8, "application/json");

            return await _httpClient.SendPost<VideoContract>("impressions", content);
        }
    }
}
