using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class ImpressionContract
    {
        public ImpressionContract()
        {
        }

        public ImpressionContract(Impression impression)
        {
            Type = impression.Type;
            UserId = impression.User.Id;
            PublicVideoId = impression.Video.PublicId;
        }

        public Impression.ImpressionType Type { get; set; }
        public string UserId { get; set; }
        public string PublicVideoId { get; set; }
    }
}
