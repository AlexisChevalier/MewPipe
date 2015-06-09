using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Services;

namespace MewPipe.Logic.Factories
{
    public class VideoServiceFactory
    {
        public IVideoApiService GetVideoApiService()
        {
            return new VideoApiService();
        }

        public IVideoWorkerService GetVideoWorkerService()
        {
            return new VideoWorkerService();
        }
    }
}
