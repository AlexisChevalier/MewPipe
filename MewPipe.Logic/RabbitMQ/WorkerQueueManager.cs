using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using RabbitMQ.Client;

namespace MewPipe.Logic.RabbitMQ
{
    public interface IWorkerQueueManager : IDisposable
    {
        IChannelQueue GetChannelQueue(WorkerQueueManager.QueueChannelIdentifier queueChannelIdentifier);
        IChannelQueueConsumer GetChannelQueueConsumer(WorkerQueueManager.QueueChannelIdentifier queueChannelIdentifier);
    }

    public class WorkerQueueManager : IWorkerQueueManager
    {
        public enum QueueChannelIdentifier
        {
            NewVideos,
            RecommendationsUpdates
        }

        private readonly IConnection _connection;
        private static readonly string HostName = ConfigurationManager.ConnectionStrings["MewPipeRabbitMQHostname"].ConnectionString;

        public WorkerQueueManager(IConnectionFactory connectionFactory = null)
        {
            var localonnectionFactory = connectionFactory ?? new ConnectionFactory() { HostName = HostName };

            Debug.Assert(localonnectionFactory != null);

            _connection = localonnectionFactory.CreateConnection();
        }

        public IChannelQueue GetChannelQueue(QueueChannelIdentifier queueChannelIdentifier)
        {

            var channelModel = _connection.CreateModel();

            var queueName = GetChannelName(queueChannelIdentifier);

            channelModel.QueueDeclare(queueName, true, false, false, null);

            return new ChannelQueue(channelModel, queueName);
        }

        public IChannelQueueConsumer GetChannelQueueConsumer(QueueChannelIdentifier queueChannelIdentifier)
        {
            var channelQueue = GetChannelQueue(queueChannelIdentifier);

            var channelModel = channelQueue.GetChannelModel();

            channelModel.BasicQos(0, 1, false);

            var consumer = new QueueingBasicConsumer(channelModel);

            channelModel.BasicConsume(channelQueue.GetQueueName(), false, consumer);

            return new ChannelQueueConsumer(consumer, channelModel, channelQueue.GetQueueName());
        }

        private static string GetChannelName(QueueChannelIdentifier queueChannelIdentifier)
        {
            switch (queueChannelIdentifier)
            {
                case QueueChannelIdentifier.NewVideos:
                {
                    return "NEW_VIDEOS";
                }
                case QueueChannelIdentifier.RecommendationsUpdates:
                {
                    return "RECOMMENDATIONS_UPDATE";
                }
                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
