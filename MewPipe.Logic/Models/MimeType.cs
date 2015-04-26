using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.Logic.Models
{
    public class MimeType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        [MaxLength(40)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(40)]
        [Index(IsUnique = true)]
        public string HttpMimeType { get; set; }
        public bool AllowedForDecoding { get; set; }
        public bool RequiredForEncoding { get; set; }
        public bool IsDefault { get; set; }
        [InverseProperty("MimeType")]
        public IEnumerable<VideoFile> VideoFiles { get; set; }
    }
}
