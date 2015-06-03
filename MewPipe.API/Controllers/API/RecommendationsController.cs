using System;
using System.Collections.Generic;
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
using WebGrease.Css.Extensions;

namespace MewPipe.API.Controllers.API
{
    public class RecommendationsController : ApiController
    {
        [HttpGet]
        [Route("api/recommendations/{videoId}")]
        public Dictionary<double, VideoContract> GetRecommendations(string videoId, int page = 0, int limit = 20)
        {
            limit = limit > 40 ? 40 : limit;

            var uow = new UnitOfWork();

            var user = ActionContext.GetUser();

            var id = ShortGuid.Decode(videoId);

            var results =
                uow.VideoRepository.GetOne(
                    v => v.Id == id, "Recommendations").Recommendations
                        .Skip(page * limit)
                        .Take(limit);

            var recommendations = results.ToDictionary(result => result.Score, result => new VideoContract(result.Video, user == null ? null : user.Id));

            return recommendations;
        }

    }
}
