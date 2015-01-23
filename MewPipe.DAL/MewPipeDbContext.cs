using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.DAL.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MewPipe.DAL
{
    public class MewPipeDbContext : IdentityDbContext<User>
    {
        public MewPipeDbContext()
            : base("MewPipeConnection", throwIfV1Schema: false)
        {
        }

        public static MewPipeDbContext Create()
        {
            return new MewPipeDbContext();
        }
    }
}
