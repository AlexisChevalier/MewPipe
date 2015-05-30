﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using MewPipe.Logic.Models.Oauth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MewPipe.Logic.Models
{
    public class User : IdentityUser
    {
        [InverseProperty("User")]
        public virtual ICollection<OauthUserTrust> OauthUserTrusts { get; set; }

        public virtual ICollection<Video> VideosSharedWithMe { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public virtual ICollection<VideoUploadToken> VideoUploadTokens { get; set; }


        public ICollection<Impression> Impressions { get; set; }

        /**
         * defaultAuthenticationType should be of type DefaultAuthenticationTypes.*
         */
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string defaultAuthenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, defaultAuthenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
