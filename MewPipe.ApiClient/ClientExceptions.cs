using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.ApiClient
{
    public class ApiForbiddenException : Exception
    {
        public ApiForbiddenException(string message) : base(message)
        {
        }

        public ApiForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
