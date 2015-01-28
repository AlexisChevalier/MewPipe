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
        public string Type { get; set; }
        public DateTime ExpirationTime { get; set; } // 1 hour, maybe ?
        public string Token { get; set; }
        public virtual OauthAccessToken AccessToken { get; set; }
    }
}
