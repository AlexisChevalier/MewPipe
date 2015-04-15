using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MewPipe.Logic.RabbitMQ
{
    public interface IChannelQueue : IDisposable
    {
        IModel GetChannelModel();
        string GetQueueName();
        void SendPersistentMessage(string message);
        void SendPersistentMessage<T>(T message);
    }

    public class ChannelQueue : IChannelQueue
    {
        private readonly IModel _model;
        private readonly string _queueName;

        public ChannelQueue(IModel channelModel, string queueName)
        {
            Debug.Assert(channelModel != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(queueName));

            _queueName = queueName;
            _model = channelModel;
        }

        public IModel GetChannelModel()
        {
            return _model;
        }

        public string GetQueueName()
        {
            return _queueName;
        }

        public void SendPersistentMessage(string message)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(message));

            var body = Encoding.UTF8.GetBytes(message);

            var properties = _model.CreateBasicProperties();
            properties.SetPersistent(true);

            _model.BasicPublish("", _queueName, properties, body);
        }

        public void SendPersistentMessage<T>(T message)
        {
            Debug.Assert(message != null);
            SendPersistentMessage(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}
