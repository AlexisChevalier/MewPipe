using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Services;
using MongoDB.Bson;

namespace MewPipe.Logic.Models
{
    public class Video
    {    
        public enum StatusTypes
        {
            Created,
            Processing,
            Published
        }

        public enum PrivacyStatusTypes
        {
            Public,
            LinkOnly,
            Private
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StatusTypes Status { get; set; }
        public PrivacyStatusTypes PrivacyStatus { get; set; }
        [InverseProperty("VideosSharedWithMe")]
        public ICollection<User> AllowedUsers { get; set; }
        [InverseProperty("Video")]
        public ICollection<VideoFile> VideoFiles { get; set; }

        public string UploadRedirectUri { get; set; }
        public string NotificationHookUri { get; set; }

        public string PublicId
        {
            get { return new ShortGuid(Id); }
        }

        public VideoFile GetVideoFile(MimeType preferedMimeType = null, QualityType preferedQuality = null)
        {
            var mimeTypeService = new VideoMimeTypeService();
            var qualityTypeService = new VideoQualityTypeService();

            if (preferedMimeType == null)
            {
                preferedMimeType = mimeTypeService.GetDefaultEncodingMimeType();
            }

            if (preferedQuality == null)
            {
                preferedQuality = qualityTypeService.GetDefaultQualityType();
            }

            var file = VideoFiles.FirstOrDefault(vf => !vf.IsOriginalFile && vf.MimeType.Id == preferedMimeType.Id && vf.QualityType.Id == preferedQuality.Id);

            if (file == null)
            {
                throw new FileNotFoundException();
            }

            return file;
        }

        public VideoFile GetOriginalFile()
        {
            return VideoFiles.FirstOrDefault(vf => vf.IsOriginalFile);
        }
    }
}
