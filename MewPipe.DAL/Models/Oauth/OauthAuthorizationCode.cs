using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.DAL.Models.Oauth
{
    public class OauthAuthorizationCode
    {
        public OauthAuthorizationCode()
        {
        }

        public OauthAuthorizationCode(User user, OauthClient oauthClient, string state, string scope, string code, string redirect_uri)
        {
            Validity = DateTime.UtcNow.AddMinutes(10);
            User = user;
            OauthClient = oauthClient;
            State = state;
            Scope = scope;
            Code = code;
            RedirectUri = redirect_uri;
        }

        public int Id { get; set; }
        public DateTime Validity { get; set; }
        public string Code { get; set; }
        public virtual User User { get; set; }
        public virtual OauthClient OauthClient { get; set; }
        public string State { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
    }
}
