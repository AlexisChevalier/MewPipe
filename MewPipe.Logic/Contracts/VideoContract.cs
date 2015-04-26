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
        }

        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}