using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.DAL.Models.Oauth
{
    public class OauthAccessToken
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Token { get; set; }
    }
}
