using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ImageService : BaseService<Image>, IDisposable
    {
        public ImageService()
        {

        }

        /// <summary>
        /// Gets an Image using the specified objectId, objectType and isMain
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="isMain"></param>
        /// <returns></returns>
        public Image Get( int objectId, string objectType, bool isMain )
        {
            return context.Images.FirstOrDefault( b => b.ObjectId == objectId && b.ObjectType == objectType && b.IsMain == isMain );
        }

        /// <summary>
        /// Gets a list of Images using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<Image> List( int objectId, string objectType )
        {
            return context.Images.Where( b => b.ObjectId == objectId && b.ObjectType == objectType ).ToList();
        }
    }
}
