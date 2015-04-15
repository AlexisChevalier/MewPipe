using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.Logic.RabbitMQ.Messages
{
    public class NewVideoMessage
    {
        public string VideoId { get; set; }

        public NewVideoMessage(string videoId)
        {
            VideoId = videoId;
        }
    }
}
