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
		void MarkVideoAsPublished(Video video);
        void RemoveVideoUploadedFile(Video video); 
	    void AddThumbnail(Video video, FileStream fileStream);
	    void AddConvertedVideo(Video video, MimeType mimeType, QualityType qualityType, FileStream fileStream);
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

			return videoService.GetVideoStream(video, originalFile.MimeType, originalFile.QualityType);
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

			videoService.RemoveFile(video, video.GetOriginalFile().MimeType, video.GetOriginalFile().QualityType);

			_unitOfWork.VideoFileRepository.Delete(file.Id);
		}

		public Video GetVideoDetails(string publicVideoId)
		{
			Debug.Assert(publicVideoId != null);

			var id = ShortGuid.Decode(publicVideoId);

			return _unitOfWork.VideoRepository.GetOne(v => v.Id == id, "VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType");
		}

	    public void AddThumbnail(Video video, FileStream fileStream)
	    {
            var thumbnailService = new ThumbnailGridFsClient();
	        thumbnailService.UploadThumbnailStream(video, fileStream);
	    }


	    public void AddConvertedVideo(Video video, MimeType mimeType, QualityType qualityType, FileStream fileStream)
        {
            var videoGridFsClient = new VideoGridFsClient();

            videoGridFsClient.UploadVideoStream(video, mimeType, qualityType, fileStream);

            var dbVideo = GetVideoDetails(video.PublicId);

            var dbMimeType = _unitOfWork.MimeTypeRepository.GetById(mimeType.Id);
            var dbQualityType = _unitOfWork.QualityTypeRepository.GetById(qualityType.Id);

            var videoFile = new VideoFile
            {
                IsOriginalFile = false,
                MimeType = dbMimeType,
                QualityType = dbQualityType,
                Video = dbVideo
            };

            dbVideo.VideoFiles.Add(videoFile);
            _unitOfWork.VideoRepository.Update(dbVideo);
            _unitOfWork.Save();
        }
	}
}