using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MewPipe.Logic.Models;

namespace MewPipe.Website.ViewModels
{
    public class ValidateUploadedVideoViewModel
    {
        public string code { get; set; }
    }

    public class EditVideoViewModel
    {
        [Required(ErrorMessage = "Invalid Video")]
        public string PublicId { get; set; }
        [Display(Name = "Video Name")]
        [Required(ErrorMessage = "Please type a valid name")]
        public string Name { get; set; }
        [Display(Name = "Video Description")]
        [Required(ErrorMessage = "Please type a valid description")]
        public string Description { get; set; }
        [Display(Name = "Privacy Mode")]
        [Required(ErrorMessage = "Please select a valid privacy mode")]
        public Video.PrivacyStatusTypes PrivacyStatus { get; set; }
    }
}