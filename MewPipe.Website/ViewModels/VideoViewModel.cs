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
    public class DeleteVideoViewModel
    {
        [Required(ErrorMessage = "Invalid Video")]
        public string PublicId { get; set; }
    }

    public class AddUserToVideoWhiteListViewModel
    {
        [Required(ErrorMessage = "Invalid Video")]
        public string PublicId { get; set; }

        [Display(Name = "User Email")]
        [Required(ErrorMessage = "Please type a valid name")]
        [EmailAddress(ErrorMessage = "Please type a valid email address")]
        public string UserEmail { get; set; }
    }

    public class SearchViewModel
    {
        public string Term { get; set; }
        public string OrderCriteria { get; set; }
        public bool OrderDesc { get; set; }
        public int Page { get; set; }

        public SearchViewModel()
        {
            Term = null;
            OrderCriteria = "date";
            OrderDesc = false;
            Page = 0;
        }

        public void Fix()
        {
            if (String.IsNullOrWhiteSpace(Term))
            {
                Term = null;
            }
            if (OrderCriteria != "date" || OrderCriteria != "goodImpressionsPercentage" || OrderCriteria != "views")
            {
                OrderCriteria = "date";
            }
            if (Page <= 0)
            {
                Page = 0;
            }
        }
    }
}