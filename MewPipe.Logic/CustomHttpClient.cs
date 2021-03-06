﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace MewPipe.Logic
{
    public interface ICustomHttpClient
    {
        Task<T> SendGet<T>(string relativeUri, string endpointOverrider = null);
        Task<T> SendDelete<T>(string relativeUri, string endpointOverrider = null);
        Task<T> SendPost<T>(string relativeUri, HttpContent data = null, string endpointOverrider = null);
        Task<T> SendPut<T>(string relativeUri, HttpContent data = null, string endpointOverrider = null);

        void SetBearerToken(string bearerToken);
    }

    public class CustomHttpClient : ICustomHttpClient
    {
        private readonly string _endpoint;
        private readonly HttpClient _httpClient;

        public CustomHttpClient(string endpoint, string bearerToken = null)
        {
            _endpoint = endpoint;
            _httpClient = new HttpClient();

            if (bearerToken != null)
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
            }
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetBearerToken(string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public async Task<T> SendGet<T>(string relativeUri, string endpointOverrider = null)
        {
            var result = await _httpClient.GetAsync((endpointOverrider ?? _endpoint) + relativeUri);

            if ((int)result.StatusCode == 481)
            {
                throw new OauthExpiredTokenException();
            }
            var stringContent = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpException((int)result.StatusCode, stringContent);
            }

            return JsonConvert.DeserializeObject<T>(stringContent);
        }

        public async Task<T> SendDelete<T>(string relativeUri, string endpointOverrider = null)
        {
            var result = await _httpClient.DeleteAsync((endpointOverrider ?? _endpoint) + relativeUri);

            if ((int)result.StatusCode == 481)
            {
                throw new OauthExpiredTokenException();
            }
            var stringContent = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpException((int)result.StatusCode, stringContent);
            }
            return JsonConvert.DeserializeObject<T>(stringContent);
        }

        public async Task<T> SendPost<T>(string relativeUri, HttpContent data = null, string endpointOverrider = null)
        {
            var result = await _httpClient.PostAsync((endpointOverrider ?? _endpoint) + relativeUri, data);

            if ((int)result.StatusCode == 481)
            {
                throw new OauthExpiredTokenException();
            }
            
            var stringContent = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpException((int)result.StatusCode, stringContent);
            }

            return JsonConvert.DeserializeObject<T>(stringContent);
        }
        public async Task<T> SendPut<T>(string relativeUri, HttpContent data = null, string endpointOverrider = null)
        {
            var result = await _httpClient.PutAsync((endpointOverrider ?? _endpoint) + relativeUri, data);

            if ((int)result.StatusCode == 481)
            {
                throw new OauthExpiredTokenException();
            }

            var stringContent = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpException((int)result.StatusCode, stringContent);
            }


            return JsonConvert.DeserializeObject<T>(stringContent);
        }

    }

    public class OauthExpiredTokenException : Exception
    {
        public OauthExpiredTokenException()
        {
        }

        public OauthExpiredTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public OauthExpiredTokenException(string message) : base(message)
        {
        }
    }
}
