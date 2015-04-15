using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.VideoWorker
{
    public class InvalidFileException : Exception
    {
        public InvalidFileException()
            : base("Invalid file")
        {

        }
        public InvalidFileException(Exception inner)
            : base("Invalid file", inner)
        {

        }
    }
}
