using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MewPipe.Logic.Models;
using MewPipe.Logic.Mongo;
using MewPipe.Logic.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.Services
{
    public interface IVideoApiService
    {
        HttpResponseMessage GetVideoHttpResponseMessage(HttpRequestMessage request, string videoId, User user);
        Video UploadVideo(HttpRequestMessage request, User user);
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

            var videoStream = GetVideoStream(videoDetails.GridFsId);

            if (IsRangeRequest(request))
            {
                return GetRangeResponse(request, videoStream, videoDetails);
            }

            return GetSimpleResponse(request, videoStream, videoDetails);
        }

        #region GetVideoHttpResponseMessage region

        private HttpResponseMessage GetRangeResponse(HttpRequestMessage request, Stream stream, Video videoDetails)
        {
            var partialResponse = request.CreateResponse(HttpStatusCode.PartialContent);
            partialResponse.Content = new ByteRangeStreamContent(stream, request.Headers.Range, videoDetails.MimeContentType);
            return partialResponse;
        }

        private HttpResponseMessage GetSimpleResponse(HttpRequestMessage request, Stream stream, Video videoDetails)
        {
            var fullResponse = request.CreateResponse(HttpStatusCode.OK);
            fullResponse.Content = new StreamContent(stream);
            fullResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(videoDetails.MimeContentType);
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

            return _unitOfWork.VideoRepository.GetById(videoId);
        }

        private bool IsUserAllowedToSeeVideo(string videoId, User user)
        {
            Debug.Assert(videoId != null);

            var videos = _unitOfWork.VideoRepository.Get(v => 
                v.Id == videoId &&
                v.Status == VideoStatus.Published &&
                (
                    v.PrivacyType == VideoPrivacyType.Public ||
                    v.PrivacyType == VideoPrivacyType.LinkOnly ||
                    v.User == user || 
                    v.AllowedUsers.Contains(user)
                )
            );

            if (videos.First().Id == videoId)
            {
                return true;
            }

            return false;
        }

        #endregion

        public Video UploadVideo(HttpRequestMessage request, User user)
        {
            //Test rights

            //Upload

            //Register on SQL

            //Return details

            return new Video();
        }
    }
}
