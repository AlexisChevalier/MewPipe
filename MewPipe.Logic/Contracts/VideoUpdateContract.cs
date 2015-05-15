using System;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class VideoUpdateContract
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
    }
}