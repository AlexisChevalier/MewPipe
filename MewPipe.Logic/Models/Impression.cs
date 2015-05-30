using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.Logic.Models
{
    public class Impression
    {
        public enum ImpressionType
        {
            [Display(Name = "Good")]
            Good,
            [Display(Name = "Bad")]
            Bad
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public ImpressionType Type { get; set; }
        public User User { get; set; }
        public Video Video { get; set; }
    }
}
