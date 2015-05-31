using System.Diagnostics;
using System.IO;
using MewPipe.Logic.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.MongoDB
{
    public interface IThumbnailGridFsClient
    {
        void UploadThumbnailStream(Video video, FileStream stream);
        MongoGridFSStream GetThumbnailReadingStream(Video video);
        void RemoveFile(Video video);
        MongoDatabase GetDatabase();
    }

    public class ThumbnailGridFsClient : IThumbnailGridFsClient
    {
        private static MongoDatabase _mongoDatabase;

        public ThumbnailGridFsClient()
        {
            if (_mongoDatabase == null)
            {
                var mongoDbManager = new MongoDbManager();

                var mongoServer = mongoDbManager.GetServerInstance();

                _mongoDatabase = mongoServer.GetDatabase("Thumbnail");
            }
        }

        public MongoGridFSStream GetThumbnailReadingStream(Video video)
        {
            Debug.Assert(video != null);
            var file = _mongoDatabase.GridFS.FindOne(video.PublicId + "_thumbnail.jpeg");

            if (file != null)
            {
                return file.OpenRead();
            }
            return null;
        }

        public void UploadThumbnailStream(Video video, FileStream stream)
        {
            _mongoDatabase.GridFS.Upload(stream, video.PublicId + "_thumbnail.jpeg", new MongoGridFSCreateOptions
            {
                Id = video.Id.ToBson(),
                ContentType = "image/jpeg"
            });
        }

        public void RemoveFile(Video video)
        {
            Debug.Assert(video != null);

            _mongoDatabase.GridFS.Delete(video.PublicId + "_thumbnail.jpeg");
        }

        public MongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }
    }
}