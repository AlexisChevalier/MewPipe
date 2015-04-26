using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.Logic.Models
{
    public class VideoUploadToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public User User { get; set; }
        public DateTime ExpirationTime { get; set; } //One day
        public string UploadRedirectUri { get; set; }
        public string NotificationHookUri { get; set; }
    }
}
