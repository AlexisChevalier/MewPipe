using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.Logic.Models
{
    public class QualityType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        [MaxLength(40)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsUploaded { get; set; }
        [InverseProperty("QualityType")]
        public IEnumerable<VideoFile> VideoFiles { get; set; }
    }
}
