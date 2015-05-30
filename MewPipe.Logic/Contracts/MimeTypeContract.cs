using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;

namespace MewPipe.Logic.Contracts
{
    public class MimeTypeContract
    {
        public MimeTypeContract()
        {
        }

        public MimeTypeContract(MimeType mimeType)
        {
            HttpMimeType = mimeType.HttpMimeType;
        }
        public string HttpMimeType { get; set; }
    }
}
