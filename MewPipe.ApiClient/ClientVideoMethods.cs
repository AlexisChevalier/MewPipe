using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Extensions;
using Newtonsoft.Json;

namespace MewPipe.ApiClient
{
    public partial class MewPipeApiClient
    {
        public async Task<VideoContract> GetVideoDetails(string publicVideoId)
        {
            return await _httpClient.SendGet<VideoContract>("videos" + "/" + publicVideoId);
        }

        public async Task<VideoUploadTokenContract> RequestVideoUploadToken(VideoUploadTokenRequestContract contract)
        {
            var content = new StringContent(JsonConvert.SerializeObject(contract), Encoding.UTF8, "application/json");

            return await _httpClient.SendPost<VideoUploadTokenContract>("videos", content);
        }
    }
}
