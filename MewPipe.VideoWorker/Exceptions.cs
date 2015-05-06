using System;

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