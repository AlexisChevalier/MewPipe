using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MewPipe.Logic.RabbitMQ
{
    public interface IChannelQueueConsumer : IDisposable
    {
        IQueueingBasicConsumer GetChannelModelConsumer();
        IModel GetChannelModel();
        string GetQueueName();
        BasicDeliverEventArgs DequeueMessage();
        QueueMessage<T> DequeueMessage<T>();
        void AcknowledgeMessage(BasicDeliverEventArgs messageDetails);
        void AcknowledgeMessage<T>(QueueMessage<T> message);
    }

    public class ChannelQueueConsumer : IChannelQueueConsumer
    {
        private readonly IModel _model;
        private readonly IQueueingBasicConsumer _consumer;
        private readonly string _queueName;

        public ChannelQueueConsumer(IQueueingBasicConsumer channelModelConsumer, IModel channelModel, string queueName)
        {
            Debug.Assert(channelModelConsumer != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(queueName));

            _model = channelModel;
            _queueName = queueName;
            _consumer = channelModelConsumer;
        }

        public IQueueingBasicConsumer GetChannelModelConsumer()
        {
            return _consumer;
        }

        public IModel GetChannelModel()
        {
            return _model;
        }

        public string GetQueueName()
        {
            return _queueName;
        }

        public BasicDeliverEventArgs DequeueMessage()
        {
            return _consumer.Queue.Dequeue();
        }

        public QueueMessage<T> DequeueMessage<T>()
        {
            return new QueueMessage<T>(_consumer.Queue.Dequeue());
        }

        public void AcknowledgeMessage(BasicDeliverEventArgs messageDetails)
        {
            _model.BasicAck(messageDetails.DeliveryTag, false);
        }

        public void AcknowledgeMessage<T>(QueueMessage<T> message)
        {
            AcknowledgeMessage(message.GetBasicDeliverEventArgs());
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}
