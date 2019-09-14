using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class UsersInSegmentViewModel
    {
        public UsersInSegmentViewModel()
        {
            //this.UsersList = new IEnumerable<UserModel>();
            this.UsersList = new List<UserModel>();
        }

        public UsersInSegmentBlock CurrentBlock { get; set; }
        public List<UserModel> UsersList { get; set; }
    }

    public class UserModel
    {
        public string Email { get; set; }
        public bool Matched { get; set; }
        public bool IsFromInsight { get; set; }
    }
}