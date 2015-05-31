using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using RabbitMQ.Client.Content;

namespace MewPipe.Logic.Contracts
{
    public class VideoContract
    {
        public VideoContract()
        {
        }

        public VideoContract(Video video, string userId = null)
        {
            var unitOfWork = new UnitOfWork();

            if (userId != null)
            {
                var impression =
                    unitOfWork.ImpressionRepository.GetOne(i => i.User.Id == userId && i.Video.Id == video.Id,
                        "User, Video");
                if (impression != null)
                {
                    UserImpression = new ImpressionContract(impression);   
                }
            }

            if (video.User != null)
            {
                User = new UserContract(video.User);
            }

            PublicId = video.PublicId;
            Name = video.Name;
            Description = video.Description;
            Seconds = video.Seconds;
            DateTimeUtc = video.DateTimeUtc;
            Status = video.Status;
            PrivacyStatus = video.PrivacyStatus;
            Views = video.Views;
            Category = new CategoryContract(video.Category);
            VideoFiles = new List<VideoFileContract>();
            Tags = String.Join(" ", video.Tags.Select(t => t.Name).ToArray());

            foreach (var videoFile in video.VideoFiles)
            {
                VideoFiles.Add(new VideoFileContract(videoFile));
            }

            PositiveImpressions =
                unitOfWork.ImpressionRepository.Count(
                    i => i.Video.Id == video.Id && i.Type == Impression.ImpressionType.Good);

            NegativeImpressions =
                unitOfWork.ImpressionRepository.Count(
                    i => i.Video.Id == video.Id && i.Type == Impression.ImpressionType.Bad);
        }

        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public long Seconds { get; set; }
        public decimal Views { get; set; }
        public decimal PositiveImpressions { get; set; }
        public decimal NegativeImpressions { get; set; }
        public UserContract User { get; set; }
        public CategoryContract Category { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public Video.StatusTypes Status { get; set; }
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
        public List<VideoFileContract> VideoFiles { get; set; }
        public ImpressionContract UserImpression { get; set; }
    }
}