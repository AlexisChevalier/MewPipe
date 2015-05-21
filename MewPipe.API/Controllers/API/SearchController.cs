using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    public class SearchController : ApiController
    {
        [HttpGet]
        [Route("api/search/videos")]
        public VideoContract[] SearchVideos(string term, string orderCriteria = "date", bool orderDesc = false,
            int page = 0, int limit = 20)
        {
            limit = limit > 40 ? 40 : limit;

            var uow = new UnitOfWork();

            var results = uow.VideoRepository.Search(v => v.PrivacyStatus == Video.PrivacyStatusTypes.Public && v.Status == Video.StatusTypes.Published && (v.Description.Contains(term) || v.Name.Contains(term)),
                GetSearchVideoOrderCriteria(orderCriteria, orderDesc), "Impressions, Tags, Category, AllowedUsers, User, VideoFiles, VideoFiles.MimeType, VideoFiles.QualityType", limit, page * limit)
                .Select(v => new VideoContract(v)).ToArray();

            return results;
        }

        private Func<IQueryable<Video>, IOrderedQueryable<Video>> GetSearchVideoOrderCriteria(string orderCriteria, bool orderDesc)
        {
            switch (orderCriteria)
            {
                case "date":
                {
                    if (orderDesc)
                    {
                        return source => source.OrderByDescending(v => v.DateTimeUtc);
                    }

                    return source => source.OrderBy(v => v.DateTimeUtc);
                }
                case "goodImpressionsPercentage":
                {
                    if (orderDesc)
                    {
                        return source => source.OrderByDescending(v =>
                        (
                            v.Impressions.Count(i => i.Type == Impression.ImpressionType.Good)
                            /
                            v.Impressions.Count
                        ) * 100);
                    }

                    return source => source.OrderBy(v =>
                        (
                            v.Impressions.Count(i => i.Type == Impression.ImpressionType.Good)
                            /
                            v.Impressions.Count
                        ) * 100);
                }
                case "views":
                {
                    if (orderDesc)
                    {
                        return source => source.OrderByDescending(v => v.Views);
                    }

                    return source => source.OrderBy(v => v.Views);
                }
            }
            return null;
        }
    }
}
