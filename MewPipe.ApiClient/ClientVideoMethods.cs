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

        public async Task<UserContract[]> GetVideoWhiteList(string publicVideoId)
        {
            return await _httpClient.SendGet<UserContract[]>("videos" + "/" + publicVideoId + "/whiteList");
        }

        public async Task<VideoContract> UpdateVideo(string publicVideoId, VideoUpdateContract contract)
        {
            var content = new StringContent(JsonConvert.SerializeObject(contract), Encoding.UTF8, "application/json");

            return await _httpClient.SendPut<VideoContract>("videos" + "/" + publicVideoId, content);
        }

        public async Task<VideoUploadTokenContract> RequestVideoUploadToken(VideoUploadTokenRequestContract contract)
        {
            var content = new StringContent(JsonConvert.SerializeObject(contract), Encoding.UTF8, "application/json");

            return await _httpClient.SendPost<VideoUploadTokenContract>("videos", content);
        }

        public async Task<VideoContract> DeleteVideo(string publicVideoId)
        {
            return await _httpClient.SendDelete<VideoContract>("videos" + "/" + publicVideoId);
        }

        public async Task<UserContract[]> RemoveUserFromWhiteList(string publicVideoId, string userId)
        {
            return await _httpClient.SendDelete<UserContract[]>("videos" + "/" + publicVideoId + "/whiteList?userId=" + userId);
        }

        public async Task<UserContract[]> AddUserToWhiteList(string publicVideoId, string userEmail)
        {
            return await _httpClient.SendPost<UserContract[]>("videos" + "/" + publicVideoId + "/whiteList?userEmail=" + userEmail);
        }
    }
}
