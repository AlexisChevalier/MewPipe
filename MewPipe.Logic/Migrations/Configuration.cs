using MewPipe.Logic.Models;
using MewPipe.Logic.Models.Oauth;

namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MewPipe.Logic.MewPipeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MewPipe.Logic.MewPipeDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Categories.AddOrUpdate(
                c => c.Name,
                new Category
                {
                    Name = "Other",
                    IsDefault = true
                },
                new Category
                {
                    Name = "News and Politics"
                },
                new Category
                {
                    Name = "Cars"
                }, 
                new Category
                {
                    Name = "Entertainment"
                },
                new Category
                {
                    Name = "Education"
                },
                new Category
                {
                    Name = "Movies"
                },
                new Category
                {
                    Name = "ONGs"
                },
                new Category
                {
                    Name = "Humor"
                },
                new Category
                {
                    Name = "Video games"
                },
                new Category
                {
                    Name = "Music"
                },
                new Category
                {
                    Name = "Science"
                },
                new Category
                {
                    Name = "People and blogs"
                },
                new Category
                {
                    Name = "Sport"
                },
                new Category
                {
                    Name = "Lifestyle"
                },
                new Category
                {
                    Name = "Travels and events"
                });

            context.OauthClients.AddOrUpdate(
                c => c.ClientId,
                new OauthClient
                {
                    ClientId = "daa2fb46-fae8-4552-898c-4f27b3fd9003",
                    ClientSecret = "207cfd69-838e-4aa0-ab0a-f456a949532c",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi consectetur dolor mi, eget faucibus metus elementum nec. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Phasellus eleifend eu elit eu auctor. Aliquam aliquam tempor nibh, at posuere neque tincidunt quis.",
                    DialogDisabled = true,
                    Name = "MewPipe Website",
                    RedirectUri = "http://mewpipe.local:44402/auth/HandleOAuthRedirect",
                    ImageSrc = "http://s.ytimg.com/yts/img/youtube_logo_stacked-vfl225ZTx.png"
                });

            context.QualityTypes.AddOrUpdate(
                q => q.Name,
                new QualityType
                {
                    Name = "Uploaded",
                    IsUploaded = true
                },
                new QualityType
                {
                    Name = "360",
                    IsDefault = true
                },
                new QualityType
                {
                    Name = "480"
                },
                new QualityType
                {
                    Name = "720"
                },
                new QualityType
                {
                    Name = "1080"
                }
                );

            context.MimeTypes.AddOrUpdate(
                m => m.HttpMimeType,
                new MimeType
                {
                    HttpMimeType = "video/mp4",
                    Name = "MP4",
                    AllowedForDecoding = true,
                    RequiredForEncoding = true,
                    IsDefault = true
                },
                new MimeType
                {
                    HttpMimeType = "video/ogg",
                    Name = "OGG",
                    AllowedForDecoding = true,
                    RequiredForEncoding = true
                },
                new MimeType
                {
                    HttpMimeType = "video/quicktime",
                    Name = "MOV",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/mp4v-es",
                    Name = "MPEG4",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/avi",
                    Name = "AVI",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/x-ms-wmv",
                    Name = "WMV",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/MP2P",
                    Name = "MPEGPS",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/x-flv",
                    Name = "FLV",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/3gpp",
                    Name = "3GPP",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                },
                new MimeType
                {
                    HttpMimeType = "video/webm",
                    Name = "WebM",
                    AllowedForDecoding = true,
                    RequiredForEncoding = false
                });
        }
    }
}
