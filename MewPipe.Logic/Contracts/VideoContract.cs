using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

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
            Views = video.Views;
            Category = new CategoryContract(video.Category);
            VideoFiles = new List<VideoFileContract>();

            foreach (var videoFile in video.VideoFiles)
            {
                VideoFiles.Add(new VideoFileContract(videoFile));
            }

            var unitOfWork = new UnitOfWork();

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
        public long Seconds { get; set; }
        public decimal Views { get; set; }
        public decimal PositiveImpressions { get; set; }
        public decimal NegativeImpressions { get; set; }
        public CategoryContract Category { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public Video.StatusTypes Status { get; set; }
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
        public List<VideoFileContract> VideoFiles { get; set; }
    }
}