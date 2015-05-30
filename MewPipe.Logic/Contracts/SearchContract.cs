using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.Logic.Contracts
{
    public class SearchContract
    {
        public SearchContract()
        {
            Results = new VideoContract[0];
        }

        public VideoContract[] Results { get; set; }
        public decimal ResultsCount { get; set; }
    }
}