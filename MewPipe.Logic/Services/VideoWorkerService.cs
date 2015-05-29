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
        MongoGridFSStream GetStreamToAddVideoThumbnail(Video video);
        MongoGridFSStream GetStreamToAddConvertedVideo(Video video, MimeType mimeType, QualityType qualityType);
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


        /// <summary>
        /// Retourne un MongoGridFSStream qui pointe sur le bon endroit de mongo ou stocker le thumbnail d'une video
        /// </summary>
        /// <param name="video">La video originale dont la video convertie provient</param>
        /// <returns>un MongoGridFSStream qui pointe sur le bon endroit de mongo ou stocker le thumbnail d'une video</returns>
        public MongoGridFSStream GetStreamToAddVideoThumbnail(Video video)
        {
            var thumbnailService = new ThumbnailGridFsClient();

            return thumbnailService.GetThumbnailWritingStream(video);
        }

        /// <summary>
        /// Retourne un MongoGridFSStream qui pointe sur le bon endroit de mongo en fonction de l'id de la video, le format et la qualité.
        /// </summary>
        /// <param name="video">La video originale dont la video covertit provient.</param>
        /// <param name="mimeType">Le format de la video convertie (valeurs: "mp4" ou "ogg")</param>
        /// <param name="qualityType">La qualité de la video convertie (valeurs: "1080" ou "720" ou "480"  ou "360")</param>
        /// <returns>Un MongoGridFSStream qui pointe sur le bon endroit de mongo en fonction de l'id de la video, le format et la qualité.</returns>

	    public MongoGridFSStream GetStreamToAddConvertedVideo(Video video, MimeType mimeType, QualityType qualityType)
	    {
            var videoGridFsClient = new VideoGridFsClient();

            var stream = videoGridFsClient.GetVideoWritingStream(video, mimeType, qualityType);

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


            return stream;
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