using System;
using System.Diagnostics;
using System.IO;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.MongoDB;
using MewPipe.Logic.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.Services
{
    public interface IVideoWorkerService
    {
        Video GetVideoDetails(string videoId);
        MongoGridFSStream GetVideoUploadedFile(Video video);
        VideoFile AddVideoFile(string videoId, MimeType mimeType, QualityType qualityType, Stream fileStream);
        void MarkVideoAsPublished(Video video);
        void RemoveVideoUploadedFile(Video video);
    }

    public class VideoWorkerService : IVideoWorkerService
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        Video IVideoWorkerService.GetVideoDetails(string videoId)
        {
            return GetVideoDetails(videoId);
        }

        public MongoGridFSStream GetVideoUploadedFile(Video video)
        {
            var videoService = new VideoGridFsClient();

            if (video == null)
            {
                throw new NullReferenceException();
            }

            var originalFile = video.GetOriginalFile();

            return videoService.GetVideoStream(new ObjectId(originalFile.GridFsId));
        }

        public VideoFile AddVideoFile(string videoId, MimeType mimeType, QualityType qualityType, Stream fileStream)
        {
            var videoService = new VideoGridFsClient();

            var video = GetVideoDetails(videoId);

            var dbMimeType = _unitOfWork.MimeTypeRepository.GetById(mimeType.Id);
            var dbQualityType = _unitOfWork.QualityTypeRepository.GetById(qualityType.Id);

            var result = videoService.CreateVideoWithStream(fileStream, videoId);

            var videoFile = new VideoFile
            {
                GridFsId = result.Id.ToString(),
                IsOriginalFile = false,
                MimeType = dbMimeType,
                QualityType = dbQualityType,
                Video = video
            };

            _unitOfWork.VideoFileRepository.Insert(videoFile);
            _unitOfWork.Save();

            return videoFile;
        }

        public void MarkVideoAsPublished(Video video)
        {
            var dbVideo = GetVideoDetails(video.PublicId);

            dbVideo.Status = Video.StatusTypes.Published;

            _unitOfWork.VideoRepository.Update(dbVideo);
            _unitOfWork.Save();
        }

        public void RemoveVideoUploadedFile(Video video)
        {
            var file = video.GetOriginalFile();

            var videoService = new VideoGridFsClient();

            videoService.RemoveFile(new ObjectId(file.GridFsId));

            _unitOfWork.VideoFileRepository.Delete(file.Id);
        }

        public Video GetVideoDetails(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var id = ShortGuid.Decode(publicVideoId);

            return _unitOfWork.VideoRepository.GetOne(v => v.Id == id, "VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType");
        }
    }
}
