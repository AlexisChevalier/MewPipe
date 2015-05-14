using System;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class VideoContract
    {
        public VideoContract()
        {
        }
        public VideoContract(Video video)
        {
            PublicId = video.PublicId;
            Name = video.Name;
            Description = video.Description;
            Seconds = video.Seconds;
            DateTimeUtc = video.DateTimeUtc;
            Status = video.Status;
            PrivacyStatus = video.PrivacyStatus;
        }

        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Seconds { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public Video.StatusTypes Status { get; set; }
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
    }
}