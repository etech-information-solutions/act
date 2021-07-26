using System;
using System.Collections.Generic;
using System.Linq;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class EstimatedLoadService : BaseService<EstimatedLoad>, IDisposable
    {
        public EstimatedLoadService()
        {

        }

        /// <summary>
        /// Gets an Estimated Load record using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public EstimatedLoad Get( int objectId, string objectType )
        {
            return context.EstimatedLoads.FirstOrDefault( a => a.ObjectId == objectId && a.ObjectType == objectType );
        }

        /// <summary>
        /// Gets a list Estimated Load records using the specified objectId and objectType
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<EstimatedLoad> List( int objectId, string objectType )
        {
            return context.EstimatedLoads.Where( a => a.ObjectId == objectId && a.ObjectType == objectType ).ToList();
        }
    }
}
