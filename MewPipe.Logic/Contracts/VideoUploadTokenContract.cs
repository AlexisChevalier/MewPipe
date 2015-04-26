using System;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class VideoUploadTokenContract
    {
        public VideoUploadTokenContract(VideoUploadToken token)
        {
            Id = token.Id;
            ExpirationTime = token.ExpirationTime;
            UploadRedirectUri = token.UploadRedirectUri;
            NotificationHookUri = token.NotificationHookUri;
        }

        public VideoUploadTokenContract()
        {
        }

        public Guid Id { get; set; }
        public DateTime ExpirationTime { get; set; } //One day
        public string UploadRedirectUri { get; set; }
        public string NotificationHookUri { get; set; }
    }

    public class VideoUploadTokenRequestContract
    {
        public VideoUploadTokenRequestContract(string uploadRedirectUri, string notificationHookUri)
        {
            UploadRedirectUri = uploadRedirectUri;
            NotificationHookUri = notificationHookUri;
        }

        public VideoUploadTokenRequestContract()
        {
        }

        public string UploadRedirectUri { get; set; }
        public string NotificationHookUri { get; set; }
    }
}