using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MewPipe.Logic.Models
{
    public enum VideoStatus
    {
        Created,
        Processing,
        Published
    }

    public enum VideoPrivacyType
    {
        Public,
        LinkOnly,
        Private
    }

    public class Video
    {
        public string Id { get; set; }
        public ObjectId GridFsId { get; set; }
        public string MimeContentType { get; set; }
        public int ContentLengthInBytes { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VideoStatus Status { get; set; }
        public VideoPrivacyType PrivacyType { get; set; }
        public ICollection<User> AllowedUsers { get; set; } 
    }
}
