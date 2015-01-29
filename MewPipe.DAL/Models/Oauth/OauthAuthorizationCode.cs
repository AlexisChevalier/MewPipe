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
