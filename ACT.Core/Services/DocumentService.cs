using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Core.Enums;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class DocumentService : BaseService<Document>, IDisposable
    {
        public DocumentService()
        {

        }

        /// <summary>
        /// Gets a list of Documents using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<Document> List( int objectId, string objectType )
        {
            return context.Documents.Where( d => d.ObjectId == objectId && d.ObjectType == objectType && d.Status == ( int ) Status.Active ).ToList();
        }

        /// <summary>
        /// Gets a document using the specified objectId, objectType and name
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Document Get( int objectId, string objectType, string name )
        {
            return context.Documents.FirstOrDefault( d => d.ObjectId == objectId && d.ObjectType == objectType && d.Name == name && d.Status == ( int ) Status.Active );
        }
    }
}
