using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.DAL.Models.Oauth
{
    public class OauthRefreshToken
    {
        public int Id { get; set; }
        public DateTime ExpirationTime { get; set; } // two weeks
        public string Token { get; set; }
        public string Scope { get; set; }
        public virtual User User { get; set; }
        public virtual OauthClient OauthClient { get; set; }
    }
}
