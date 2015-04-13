using System.Configuration;
using MongoDB.Driver;

namespace MewPipe.Logic.Mongo
{
    public interface IMongoDbManager
    {
        MongoServer GetServerInstance();
    }

    public class MongoDbManager : IMongoDbManager
    {
        private static MongoServer _mongoServer;

        public MongoServer GetServerInstance()
        {
            if (_mongoServer == null)
            {
                var client = new MongoClient(ConfigurationManager.ConnectionStrings["MewPipeMongoConnection"].ConnectionString);
                _mongoServer = client.GetServer();
            }

            return _mongoServer;
        }
    }
}
