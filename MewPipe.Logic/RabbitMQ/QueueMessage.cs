using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client.Events;

namespace MewPipe.Logic.RabbitMQ
{
    public interface IQueueMessage<out T>
    {
        BasicDeliverEventArgs GetBasicDeliverEventArgs();
        string GetUtf8MessageData();
        T GetMessageData();
    }

    public class QueueMessage<T> : IQueueMessage<T>
    {
        private readonly BasicDeliverEventArgs _basicDeliverEventArgs;

        public QueueMessage(BasicDeliverEventArgs args)
        {
            Debug.Assert(args != null);

            _basicDeliverEventArgs = args;
        }

        public string GetUtf8MessageData()
        {
            return Encoding.UTF8.GetString(_basicDeliverEventArgs.Body);
        }

        public T GetMessageData()
        {
            return JsonConvert.DeserializeObject<T>(GetUtf8MessageData());
        }

        public BasicDeliverEventArgs GetBasicDeliverEventArgs()
        {
            return _basicDeliverEventArgs;
        }
    }
}
