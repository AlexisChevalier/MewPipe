using System;
using System.Diagnostics;
using System.IO;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.MongoDB
{
    public interface IVideoGridFsClient
    {
        MongoGridFSStream GetVideoWritingStream(Video video, MimeType mimeType, QualityType qualityType);
        MongoGridFSStream GetVideoStream(Video video, MimeType mimeType, QualityType qualityType);
        void RemoveFile(Video video, MimeType mimeType, QualityType qualityType);
        string GetFileName(Video video, MimeType mimeType, QualityType qualityType);
        string GetFileName(Guid videoId, MimeType mimeType, QualityType qualityType);
        void UploadVideoStream(Video video, MimeType mimeType, QualityType qualityType, FileStream stream);
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

        public void UploadVideoStream(Video video, MimeType mimeType, QualityType qualityType, FileStream stream)
        {
            _mongoDatabase.GridFS.Upload(stream, GetFileName(video, mimeType, qualityType), new MongoGridFSCreateOptions
            {
                ContentType = mimeType.HttpMimeType
            });
        }

        public MongoGridFSStream GetVideoWritingStream(Video video, MimeType mimeType, QualityType qualityType)
        {
            _mongoDatabase.GridFS.Create(GetFileName(video, mimeType, qualityType), new MongoGridFSCreateOptions
            {
                ContentType = mimeType.HttpMimeType
            });

            return _mongoDatabase.GridFS.OpenWrite(GetFileName(video, mimeType, qualityType));
        }

        public MongoGridFSStream GetVideoStream(Video video, MimeType mimeType, QualityType qualityType)
        {

            return _mongoDatabase.GridFS.FindOne(GetFileName(video, mimeType, qualityType)).OpenRead();
        }

        public void RemoveFile(Video video, MimeType mimeType, QualityType qualityType)
        {
            _mongoDatabase.GridFS.Delete(GetFileName(video, mimeType, qualityType));
        }

        public MongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }

        public string GetFileName(Video video, MimeType mimeType, QualityType qualityType)
        {
            return GetFileName(video.Id, mimeType, qualityType);
        }

        public string GetFileName(Guid videoId, MimeType mimeType, QualityType qualityType)
        {
            var name = String.Format("{0}-VideoFile-{1}-{2}", new ShortGuid(videoId), mimeType.HttpMimeType, qualityType.Name);

            name = name.Replace("/", "_");

            return name;
        }
    }
}