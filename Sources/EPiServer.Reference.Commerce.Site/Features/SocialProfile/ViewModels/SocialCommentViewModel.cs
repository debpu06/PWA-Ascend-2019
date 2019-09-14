using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.SocialProfile.ViewModels
{
    public class SocialCommentViewModel
    {
        public SocialCommentViewModel()
        {

        }

        public SocialCommentViewModel(string user)
        {
            this.User = user;
        }

        [Required]
        public string User { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide your nickname.")]
        public string Nickname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please add a comment.")]
        public string Body { get; set; }
    }
}