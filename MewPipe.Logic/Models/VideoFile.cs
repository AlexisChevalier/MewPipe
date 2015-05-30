using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Services;
using MongoDB.Bson;

namespace MewPipe.Logic.Models
{

    public class VideoFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public MimeType MimeType { get; set; }
        public QualityType QualityType { get; set; }
        public Video Video { get; set; }
        public bool IsOriginalFile { get; set; }
    }
}
