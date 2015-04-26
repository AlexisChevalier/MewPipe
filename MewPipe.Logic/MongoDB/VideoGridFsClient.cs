using System.Diagnostics;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.MongoDB
{
    public interface IVideoGridFsClient
    {
        MongoGridFSFileInfo CreateVideoWithStream(Stream fileInputStream, string fileName);
        MongoGridFSStream GetVideoStream(ObjectId objectId);
        void RemoveFile(ObjectId objectId);
        MongoDatabase GetDatabase();
    }

    public class VideoGridFsClient : IVideoGridFsClient
    {
        private static MongoDatabase _mongoDatabase;

        public VideoGridFsClient()
        {
            if (_mongoDatabase == null)
            {
                var mongoDbManager = new MongoDbManager();

                var mongoServer = mongoDbManager.GetServerInstance();

                _mongoDatabase = mongoServer.GetDatabase("Video");
            }
        }

        public MongoGridFSFileInfo CreateVideoWithStream(Stream fileInputStream, string fileName)
        {
            Debug.Assert(fileInputStream != null);
            Debug.Assert(fileName != null);

            return _mongoDatabase.GridFS.Upload(fileInputStream, fileName);
        }

        public MongoGridFSStream GetVideoStream(ObjectId objectId)
        {
            Debug.Assert(objectId != null);

            return _mongoDatabase.GridFS.FindOneById(objectId).OpenRead();
        }

        public void RemoveFile(ObjectId objectId)
        {
            Debug.Assert(objectId != null);

            _mongoDatabase.GridFS.DeleteById(objectId);
        }

        public MongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }
    }
}