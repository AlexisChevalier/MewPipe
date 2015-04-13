using System;

namespace MewPipe.Logic.Models.Oauth
{
    public class OauthAuthorizationCode
    {
        public int Id { get; set; }
        public DateTime ExpirationTime { get; set; } // 10 minutes
        public string Code { get; set; }
        public virtual User User { get; set; }
        public virtual OauthClient OauthClient { get; set; }
        public string State { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
    }
}
