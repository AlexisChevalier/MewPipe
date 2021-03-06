﻿using System.ComponentModel.DataAnnotations;

namespace MewPipe.Accounts.ViewModels
{
    public class AuthorizeRequestViewModel
    {

        [Required(AllowEmptyStrings = false)]
        public string response_type { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string client_id { get; set; }
        public string state { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string redirect_uri { get; set; }
        public string scope { get; set; }
    }

    public class DialogRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string response_type { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string client_id { get; set; }
        public string state { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string redirect_uri { get; set; }
        public string scope { get; set; }
    }

    public class DialogViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Decision { get; set; }
        public string Scope { get; set; }
    }

    public class AccessTokenRequestViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string grant_type { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string code { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string redirect_uri { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string client_id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string client_secret { get; set; }
        public string scope { get; set; }
        public string state { get; set; }
    }

    public class RefreshTokenRequestViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string client_id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string client_secret { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string grant_type { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string refresh_token { get; set; }
        public string scope { get; set; }
    }


    public class AccessTokenResponseViewModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
    }
}