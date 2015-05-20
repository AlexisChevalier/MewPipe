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

namespace MewPipe.ApiClient
{
    public partial class MewPipeApiClient
    {
        public async Task<CategoryContract[]> GetVideoCategories()
        {
            return await _httpClient.SendGet<CategoryContract[]>("categories");
        }
    }
}
