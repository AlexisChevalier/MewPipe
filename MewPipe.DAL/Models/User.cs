using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MewPipe.DAL.Models
{
    public class User : IdentityUser
    {
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
