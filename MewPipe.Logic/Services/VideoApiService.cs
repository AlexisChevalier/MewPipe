using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.MongoDB;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.Services
{
    public interface IVideoApiService
    {
        HttpResponseMessage GetVideoHttpResponseMessage(HttpRequestMessage request, string videoId, User user);
        Task<Video> UploadVideoFromMultipartRequest(HttpRequestMessage request, string tokenId);
        VideoUploadToken GenerateVideoUploadToken(VideoUploadTokenRequestContract request, User user);
        VideoUploadToken GetVideoUploadToken(string tokenId, User user);
        Video GetVideo(string videoId, User user);
    }

    public class VideoApiService : IVideoApiService
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public HttpResponseMessage GetVideoHttpResponseMessage(HttpRequestMessage request, string videoId, User user)
        {
            Debug.Assert(videoId != null);

            var videoDetails = GetVideoDetails(videoId);

            if (videoDetails == null)
            {
                throw new HttpException(404, "Video not found");
            }

            if (!IsUserAllowedToSeeVideo(videoId, user))
            {
                throw new HttpException(401, "Unauthorized");
            }

            var file = videoDetails.GetVideoFile();

            var videoStream = GetVideoStream(new ObjectId(file.GridFsId));

            if (IsRangeRequest(request))
            {
                return GetRangeResponse(request, videoStream, file);
            }

            return GetSimpleResponse(request, videoStream, file);
        }

        public async Task<Video> UploadVideoFromMultipartRequest(HttpRequestMessage request, string tokenId)
        {
            //Test token
            var token = GetVideoUploadTokenAndUserForId(tokenId);

            if (!IsTokenValid(token))
            {
                throw new HttpException(401, "Unauthorized");
            }

            //Upload

            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpException(406, "Content-Type must me multipart/form-data");
            }

            var streamProvider = new MongoDbMultipartStreamProvider();

            try
            {
                await request.Content.ReadAsMultipartAsync(streamProvider);
            }
            catch (IOException e)
            {
                var httpException = e.InnerException.InnerException;
                throw httpException;
            }
            catch (Exception)
            {
                throw new HttpException(500, "Error while uploading video. Please try again.");
            }

            var details = streamProvider.VideoOptions;
            var video = new Video
            {
                AllowedUsers = new List<User>(),
                Description = null,
                Name = null,
                PrivacyStatus = Video.PrivacyStatusTypes.Private,
                Status = Video.StatusTypes.Processing,
                User = token.User,
                VideoFiles = new List<VideoFile>(),
                NotificationHookUri = token.NotificationHookUri,
                UploadRedirectUri = token.UploadRedirectUri
            };

            var mimeTypeService = new VideoMimeTypeService(_unitOfWork);
            var qualityTypeService = new VideoQualityTypeService(_unitOfWork);

            video.VideoFiles.Add(new VideoFile
                {
                    Video = video,
                    GridFsId = details.Id.ToString(),
                    IsOriginalFile = true,
                    MimeType = mimeTypeService.GetAllowedMimeTypeForDecoding(details.ContentType),
                    QualityType = qualityTypeService.GetUploadingQualityType()
                });

            _unitOfWork.VideoRepository.Insert(video);
            _unitOfWork.Save();

            RemoveVideoUploadToken(token);

            using (var workerQueueManager = new WorkerQueueManager())
            {
                using (var channelQueue =
                    workerQueueManager.GetChannelQueue(WorkerQueueManager.QueueChannelIdentifier.NewVideos))
                {
                    channelQueue.SendPersistentMessage(new NewVideoMessage(video.Id.ToString()));
                }
            }

            return video;
        }

        public VideoUploadToken GetVideoUploadToken(string tokenId, User user)
        {
            var token = GetVideoUploadTokenAndUserForId(tokenId);

            if (!IsTokenValid(token))
            {
                throw new HttpException(401, "Unauthorized");
            }
            return token;
        }

        public Video GetVideo(string videoId, User user)
        {
            Debug.Assert(videoId != null);

            var videoDetails = GetVideoDetails(videoId);

            if (videoDetails == null)
            {
                throw new HttpException(404, "Video not found");
            }

            if (!IsUserAllowedToSeeVideo(videoId, user))
            {
                throw new HttpException(401, "Unauthorized");
            }

            return videoDetails;
        }

        public VideoUploadToken GenerateVideoUploadToken(VideoUploadTokenRequestContract request, User user)
        {
            Debug.Assert(request != null);
            Debug.Assert(user != null);

            if (request == null || user == null)
            {
                throw new HttpException(400, "Invalid parameters");
            }

            user = _unitOfWork.UserRepository.GetById(user.Id);

            var token = new VideoUploadToken
            {
                ExpirationTime = DateTime.UtcNow.AddDays(1),
                User = user,
                NotificationHookUri = request.NotificationHookUri,
                UploadRedirectUri = request.UploadRedirectUri
            };

            user.VideoUploadTokens.Add(token);

            _unitOfWork.Save();

            return token;
        }


        #region helpers

        private HttpResponseMessage GetRangeResponse(HttpRequestMessage request, Stream stream, VideoFile videoFile)
        {
            var partialResponse = request.CreateResponse(HttpStatusCode.PartialContent);
            partialResponse.Content = new ByteRangeStreamContent(stream, request.Headers.Range, videoFile.MimeType.HttpMimeType);
            return partialResponse;
        }

        private HttpResponseMessage GetSimpleResponse(HttpRequestMessage request, Stream stream, VideoFile videoFile)
        {
            var fullResponse = request.CreateResponse(HttpStatusCode.OK);
            fullResponse.Content = new StreamContent(stream);
            fullResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(videoFile.MimeType.HttpMimeType);
            return fullResponse;
        }

        private bool IsRangeRequest(HttpRequestMessage request)
        {
            Debug.Assert(request != null);

            if (request.Headers.Range != null)
            {
                return true;
            }

            return false;
        }

        private MongoGridFSStream GetVideoStream(ObjectId gridVideoId)
        {
            Debug.Assert(gridVideoId != null);

            var videoService = new VideoGridFsClient();
            return videoService.GetVideoStream(gridVideoId);
        }

        private Video GetVideoDetails(string videoId)
        {
            Debug.Assert(videoId != null);

            var id = ShortGuid.Decode(videoId);

            return _unitOfWork.VideoRepository.GetOne(v => v.Id == id, "VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType");
        }

        private bool IsUserAllowedToSeeVideo(string videoId, User user)
        {
            Debug.Assert(videoId != null);

            var videoGuid = ShortGuid.Decode(videoId);

            var videos = _unitOfWork.VideoRepository.Get(v =>
                v.Id == videoGuid &&
                v.Status == Video.StatusTypes.Published &&
                (
                    v.PrivacyStatus == Video.PrivacyStatusTypes.Public ||
                    v.PrivacyStatus == Video.PrivacyStatusTypes.LinkOnly ||
                    v.User == user ||
                    v.AllowedUsers.Contains(user)
                )
            ).ToArray();

            if (!videos.Any())
            {
                return false;
            }

            if (videos.First().PublicId == videoId)
            {
                return true;
            }

            return false;
        }

        private VideoUploadToken GetVideoUploadTokenAndUserForId(string tokenId)
        {
            Debug.Assert(tokenId != null);

            var guid = Guid.Parse(tokenId);
            var token = _unitOfWork.VideoUploadTokenRepository.GetOne(t => t.Id == guid, "User");

            return token;
        }

        private void RemoveVideoUploadToken(VideoUploadToken token)
        {
            Debug.Assert(token != null);

            _unitOfWork.VideoUploadTokenRepository.Delete(token);
        }

        private bool IsTokenValid(VideoUploadToken token)
        {
            if (token == null)
            {
                return false;
            }

            if (token.User == null)
            {
                return false;
            }

            if (token.ExpirationTime < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
