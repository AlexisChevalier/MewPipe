using System.Collections.Generic;
using MewPipe.Logic.Models;

namespace MewPipe.DataFeeder.Entities
{
	public class MewPipeVideo
	{
		public string FilePath { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public long Views { get; set; }
		public string Category { get; set; }
		public string Author { get; set; }
		public IEnumerable<Impression> Impressions { get; set; }
	}
}