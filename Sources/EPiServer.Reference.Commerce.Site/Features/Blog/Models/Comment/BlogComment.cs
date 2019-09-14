using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Comment
{
    /// <summary>
    /// The BlogComment class describes a comment model used by the BlogItem site
    /// </summary>
    public class BlogComment
    {    
        /// <summary>
        /// The comment author username.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The comment author email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The comment body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The reference to the target the comment applies to.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The date/time the comment was created at.
        /// </summary>
        public DateTime Created { get; set; }
    }


    public class BlogCommentExtension
    { 
        public BlogCommentExtension(string email)
        {
            Email = email;
        }

        /// <summary>
        /// The comment author email.
        /// </summary>
        public string Email { get; set; }
    }
}