using System;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class VideoFileContract
    {
        public VideoFileContract()
        {
        }
        public VideoFileContract(VideoFile videoFile)
        {
            QualityType = new QualityTypeContract(videoFile.QualityType);
            MimeType = new MimeTypeContract(videoFile.MimeType);
        }

        public QualityTypeContract QualityType { get; set; }
        public MimeTypeContract MimeType { get; set; }
    }
}