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
            [Display(Name = "Created")]
            Created,
            [Display(Name = "Converted")]
            Processing,
            [Display(Name = "Published")]
            Published
        }

        public enum PrivacyStatusTypes
        {
            [Display(Name = "Public")]
            Public,
            [Display(Name = "Link only")]
            LinkOnly,
            [Display(Name = "Private")]
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
        public ICollection<User> AllowedUsers { get; set; }
        public ICollection<VideoFile> VideoFiles { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Impression> Impressions { get; set; }
        public ICollection<Recommendation> Recommendations { get; set; }
        public Category Category { get; set; }

        public DateTime DateTimeUtc { get; set; }
        public long Seconds { get; set; }
        public decimal Views { get; set; }

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
