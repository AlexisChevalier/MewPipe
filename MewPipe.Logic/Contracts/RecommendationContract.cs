using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using RabbitMQ.Client.Content;

namespace MewPipe.Logic.Contracts
{
    public class RecommendationContract
    {
        public RecommendationContract()
        {
        }

        public RecommendationContract(VideoContract video, double score)
        {
            Video = video;
            Score = score;
        }

        public VideoContract Video { get; set; }
        public double Score { get; set; }
    }
}