using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Web;
using MewPipe.Logic.Models;
using MewPipe.Logic.Services;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace MewPipe.Logic.MongoDB
{
    public class MongoDbMultipartStreamProvider : MultipartStreamProvider
    {
        //http://stackoverflow.com/questions/15842496/is-it-possible-to-override-multipartformdatastreamprovider-so-that-is-doesnt-sa
        //http://www.flapstack.com/async-file-uploads-with-web-api-mongohnoyoudidnt/
        //http://www.w3.org/TR/html401/interact/forms.html#h-17.13.4

        private static readonly Random Random = new Random();
        private readonly int _maximumUploadTentatives;
        private readonly IVideoGridFsClient _videoGridFsClient;
        private readonly IVideoMimeTypeService _videoMimeTypeService;
        private const int MaxRequestSizeInBytes = 524296192; //500 MB + 8KB for request

        public MongoGridFSCreateOptions VideoOptions { get; private set; }
        public MimeType VideoMimeType { get; private set; }

        public MongoDbMultipartStreamProvider(int maximumUploadTentatives = 3)
        {
            _maximumUploadTentatives = maximumUploadTentatives;
            _videoGridFsClient = new VideoGridFsClient();
            _videoMimeTypeService = new VideoMimeTypeService();
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {

            if (parent == null || headers == null)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "Invalid request");
            }

            if (Contents.Count > 1)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "You can only send one video at a time !");
            }
            
            if (parent.Headers.ContentLength > MaxRequestSizeInBytes) // 500mb + 1mb (http request size)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "Your request is over the maximum request size (which is 524296192 bytes)");
            }

            var contentDisposition = headers.ContentDisposition;

            if (contentDisposition == null)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "'Content-Disposition' header field in MIME multipart body part not found.");
            }

            if (string.IsNullOrEmpty(contentDisposition.FileName))
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "'Content-Disposition' header field in MIME multipart body part doesn't precise the filename, this request must only contains one single file and nothing else.");
            }

            var contentType = headers.ContentType;

            if (contentType == null)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "'Content-Type' header field in MIME multipart body part not found.");
            }

            VideoMimeType = _videoMimeTypeService.GetAllowedMimeTypeForDecoding(contentType.MediaType);

            if (VideoMimeType == null)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.BadRequest, "Mime type " + contentType.MediaType + " is not allowed on our service");
            }

            VideoOptions = new MongoGridFSCreateOptions
            {
                Id = ObjectId.GenerateNewId(),
                UploadDate = DateTime.UtcNow,
                ContentType = VideoMimeType.HttpMimeType
            };

            try
            {
                return TryGetStream(contentDisposition.FileName, VideoOptions);
            }
            catch (Exception)
            {
                throw new MongoDbMultipartStreamProviderException(HttpStatusCode.InternalServerError, "Unexpected server error. Please try again.");
            }
        }

        private MongoGridFSStream TryGetStream(string filename, MongoGridFSCreateOptions options)
        {
            var tentative = 0;

            while(true)
            {
                tentative++;
                try
                {
                    return _videoGridFsClient.GetDatabase().GridFS.Create(filename, options);
                }
                catch (Exception)
                {
                    if (tentative >= _maximumUploadTentatives)
                    {
                        throw;
                    }
                    Thread.Sleep(GetRandomWaitingTime());
                }
            }
        }

        private int GetRandomWaitingTime()
        {
            return Random.Next(100, 300);
        }
    }

    public class MongoDbMultipartStreamProviderException : Exception
    {
        public MongoDbMultipartStreamProviderException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; set; }

        public MongoDbMultipartStreamProviderException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}