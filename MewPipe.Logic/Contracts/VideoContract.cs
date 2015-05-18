using System;
using System.Collections.Generic;
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

            VideoFiles = new List<VideoFileContract>();

            foreach (var videoFile in video.VideoFiles)
            {
                VideoFiles.Add(new VideoFileContract(videoFile));
            }
        }

        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Seconds { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public Video.StatusTypes Status { get; set; }
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
        public List<VideoFileContract> VideoFiles { get; set; }
    }
}