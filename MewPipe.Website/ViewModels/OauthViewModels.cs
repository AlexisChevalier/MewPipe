using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MewPipe.Website.ViewModels
{
    public class DialogViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Decision { get; set; }
        [Required]
        public string Scope { get; set; }
    }

    public class AccessTokenRequestViewModel
    {
        public string grant_type { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
        public string state { get; set; }
    }

    public class AccessTokenResponseViewModel
    {
        
    }

    public class RefreshTokenRequestViewModel
    {
        public string grant_type { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
    }

    public class RefreshTokenResponseViewModel
    {

    }
}