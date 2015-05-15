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
using System.Web.Http;
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
		Video[] GetUserVideos(User user);
        Video UpdateVideo(string videoId, User user, VideoUpdateContract contract);
        UserContract[] GetVideoWhiteList(string videoId, User user);
        UserContract[] AddUserToWhiteList(string videoId, string usertoAddEmail, User user);
        UserContract[] RemoveUserFromWhiteList(string videoId, string usertoAddId, User user);
	    Video DeleteVideo(string videoId, User user);
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
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
			}

			if (!IsUserAllowedToSeeVideo(videoId, user))
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
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
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
			}

			//Upload

			if (!request.Content.IsMimeMultipartContent())
			{
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotAcceptable,
                    Content = new StringContent("Content-Type must me multipart/form-data")
                });
			}

			var streamProvider = new MongoDbMultipartStreamProvider();

			try
			{
				await request.Content.ReadAsMultipartAsync(streamProvider);
			}
			catch (IOException e)
			{
				var httpException = e.InnerException.InnerException as MongoDbMultipartStreamProviderException;
			    if (httpException == null)
			    {
			        throw;
			    }

                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = httpException.HttpStatusCode,
                    Content = new StringContent(httpException.Message)
                });
			}
			catch (Exception)
			{
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Error while uploading video. Please try again.")
                });
			}

			var details = streamProvider.VideoOptions;
			var video = new Video
			{
				AllowedUsers = new List<User>(),
				Description = "",
				Name = "New Video",
				PrivacyStatus = Video.PrivacyStatusTypes.Private,
				Status = Video.StatusTypes.Processing,
				User = token.User,
				VideoFiles = new List<VideoFile>(),
				NotificationHookUri = token.NotificationHookUri,
				UploadRedirectUri = token.UploadRedirectUri,
                DateTimeUtc = DateTime.UtcNow
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
					channelQueue.SendPersistentMessage(new NewVideoMessage(video.PublicId));
				}
			}

			return video;
		}

		public VideoUploadToken GetVideoUploadToken(string tokenId, User user)
		{
			var token = GetVideoUploadTokenAndUserForId(tokenId);

			if (!IsTokenValid(token))
			{
			    throw new HttpResponseException(new HttpResponseMessage
			    {
			        StatusCode = HttpStatusCode.Unauthorized,
			        Content = new StringContent("Unauthorized")
			    });
			}
			return token;
		}

		public Video GetVideo(string videoId, User user)
		{
			Debug.Assert(videoId != null);

			var videoDetails = GetVideoDetails(videoId);

			if (videoDetails == null)
			{
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Video not found")
                });
			}

			if (!IsUserAllowedToSeeVideo(videoId, user))
			{
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
			}

			return videoDetails;
		}

	    public Video[] GetUserVideos(User user)
	    {
	        return GetUserVideos(user.Id);
	    }

	    public Video UpdateVideo(string videoId, User user, VideoUpdateContract contract)
	    {
	        var video = GetVideoDetails(videoId);

	        if (video.User.Id != user.Id)
	        {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
	        }

	        video.Name = contract.Name;
	        video.Description = contract.Description;
	        if (contract.PrivacyStatus != Video.PrivacyStatusTypes.Private)
	        {
	            video.AllowedUsers = new List<User>();
	        }
	        video.PrivacyStatus = contract.PrivacyStatus;

            //TODO: Remove from search engine

            _unitOfWork.VideoRepository.Update(video);
            _unitOfWork.Save();
	        return video;
	    }

	    public UserContract[] GetVideoWhiteList(string videoId, User user)
	    {
            var video = GetVideoDetails(videoId);

            if (video.User.Id != user.Id)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
            }

	        return video.AllowedUsers.Select(u => new UserContract(u)).ToArray();
	    }

	    public UserContract[] AddUserToWhiteList(string videoId, string usertoAddEmail, User user)
	    {
            var video = GetVideoDetails(videoId);

            if (video.User.Id != user.Id)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
            }

            if (video.PrivacyStatus != Video.PrivacyStatusTypes.Private)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Video is not in private mode")
                });
            }

	        var userToAdd = _unitOfWork.UserRepository.GetOne(u => u.Email == usertoAddEmail);

	        if (userToAdd == null)
	        {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("User not found")
                });
	        }

	        if (video.AllowedUsers == null)
	        {
	            video.AllowedUsers = new List<User>();
	        }

            video.AllowedUsers.Add(userToAdd);

            _unitOfWork.VideoRepository.Update(video);
            _unitOfWork.Save();

            return video.AllowedUsers.Select(u => new UserContract(u)).ToArray();
	    }

	    public UserContract[] RemoveUserFromWhiteList(string videoId, string usertoAddId, User user)
	    {
            var video = GetVideoDetails(videoId);

            if (video.User.Id != user.Id)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
            }

            if (video.PrivacyStatus != Video.PrivacyStatusTypes.Private)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Video is not in private mode")
                });
            }

            var userToRemove = video.AllowedUsers.FirstOrDefault(u => u.Id == usertoAddId);

            if (userToRemove == null)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("User not found")
                });
            }

	        if (video.AllowedUsers == null)
	        {
	            video.AllowedUsers = new List<User>();
	        }
	        else
	        {
                video.AllowedUsers.Remove(userToRemove);
            }
            _unitOfWork.VideoRepository.Update(video);
            _unitOfWork.Save();

            return video.AllowedUsers.Select(u => new UserContract(u)).ToArray();
	    }

	    public Video DeleteVideo(string videoId, User user)
	    {
            var video = GetVideoDetails(videoId);
	        var videoGridFsClient = new VideoGridFsClient();

            if (video.User.Id != user.Id)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized")
                });
            }

            //TODO: remove from search engine

            video.Status = Video.StatusTypes.Processing;

            _unitOfWork.VideoRepository.Update(video);
            _unitOfWork.Save();

	        foreach (var videoFile in video.VideoFiles)
	        {
                videoGridFsClient.RemoveFile(new ObjectId(videoFile.GridFsId));
	        }

	        _unitOfWork.VideoRepository.Delete(video);
	        _unitOfWork.Save();

            //TODO: This has probably failed because i'm a noob with the constraints in Entity Framework.
            //TODO: We will need to check this

	        return video;
	    }

	    public VideoUploadToken GenerateVideoUploadToken(VideoUploadTokenRequestContract request, User user)
		{
			Debug.Assert(request != null);
			Debug.Assert(user != null);

			if (request == null || user == null)
			{
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Invalid Parameters")
                });
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

        private Video[] GetUserVideos(string userId)
        {
            Debug.Assert(userId != null);

            return _unitOfWork.VideoRepository.Get(v => v.User.Id == userId, q => q.OrderBy(v => v.DateTimeUtc), "VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType").ToArray();
        }

		private Video GetVideoDetails(string videoId)
		{
			Debug.Assert(videoId != null);

			var id = ShortGuid.Decode(videoId);

            return _unitOfWork.VideoRepository.GetOne(v => v.Id == id, "AllowedUsers, User, VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType");
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
					v.User.Id == user.Id ||
					v.AllowedUsers.Any(u => u.Id == user.Id)
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