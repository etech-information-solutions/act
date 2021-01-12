using System;
using System.Collections.Generic;
using System.Linq;

using ACT.Core.Enums;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class CommentService : BaseService<Comment>, IDisposable
    {
        public CommentService()
        {

        }

        /// <summary>
        /// Gets a list of Documents using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<Comment> List( int objectId, string objectType )
        {
            return context.Comments.Include( "User" ).Where( d => d.ObjectId == objectId && d.ObjectType == objectType && d.Status == ( int ) Status.Active ).ToList();
        }
    }
}
