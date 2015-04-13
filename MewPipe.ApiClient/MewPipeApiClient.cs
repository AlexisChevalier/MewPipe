using System;
using MewPipe.Logic;

namespace MewPipe.ApiClient
{


    public partial class MewPipeApiClient 
    {
        private readonly string _endpoint;
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly ICustomHttpClient _httpClient;

        public MewPipeApiClient(string endpoint, string clientId, string clientSecret, string bearerToken = null, ICustomHttpClient httpClient = null)
        {
            _endpoint = endpoint;
            _clientId = clientId;
            _clientSecret = clientSecret;

            _httpClient = httpClient ?? new CustomHttpClient(_endpoint, bearerToken);
        }

        public void SetBearerToken(string bearerToken)
        {
            _httpClient.SetBearerToken(bearerToken);
        }
    }
}
