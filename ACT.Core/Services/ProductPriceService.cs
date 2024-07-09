using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ProductPriceService : BaseService<ProductPrice>, IDisposable
    {
        public ProductPriceService()
        {
        }

        public List<ProductPrice> GetProductPrices( int productId, DateTime? date = null )
        {
            List<object> parameters = new List<object>()
            {
                new SqlParameter("@productId", productId),
                new SqlParameter("@date", date ?? DateTime.Now),
                new SqlParameter("@activeStatus", (int)Status.Active)
            };

            string query = @"SELECT pp.*
                             FROM [dbo].[ProductPrice] pp
                             WHERE pp.ProductId = @productId
                             AND pp.Status = @activeStatus
                             AND (pp.FromDate IS NULL OR pp.FromDate <= @date)
                             ORDER BY pp.FromDate DESC";

            List<ProductPrice> productPrices = context.Database.SqlQuery<ProductPrice>( query, parameters.ToArray() ).ToList();

            return productPrices;
        }

        public Dictionary<int, decimal> GetProductRates( int productId, DateTime? date = null )
        {
            date = date ?? DateTime.Now;

            var parameters = new List<object>
            {
                new SqlParameter("@productId", productId),
                new SqlParameter("@date", date)
            };

            var query = @"
                        SELECT pp.Type, pp.Rate
                        FROM [dbo].[ProductPrice] pp
                        WHERE pp.ProductId = @productId
                          AND pp.Status = 1
                          AND pp.FromDate <= @date
                          AND pp.FromDate = (
                              SELECT MAX(FromDate)
                              FROM [dbo].[ProductPrice]
                              WHERE ProductId = pp.ProductId
                                AND Type = pp.Type
                                AND Status = 1
                                AND FromDate <= @date
                          )";

            var results = context.Database.SqlQuery<ProductPriceResult>( query, parameters.ToArray() ).ToList();

            return results.ToDictionary( r => r.Type, r => r.Rate );
        }

        public ProductPrice GetLatestProductPrice( int productId, ProductPriceType priceType )
        {
            List<object> parameters = new List<object>()
            {
                new SqlParameter("@productId", productId),
                new SqlParameter("@priceType", (int)priceType),
                new SqlParameter("@activeStatus", (int)Status.Active)
            };

            string query = @"SELECT TOP 1 pp.*
                             FROM [dbo].[ProductPrice] pp
                             WHERE pp.ProductId = @productId
                             AND pp.Type = @priceType
                             AND pp.Status = @activeStatus
                             ORDER BY pp.FromDate DESC, pp.CreatedOn DESC";

            ProductPrice latestPrice = context.Database.SqlQuery<ProductPrice>( query, parameters.ToArray() ).FirstOrDefault();

            return latestPrice;
        }

        public class ProductPriceResult
        {
            public int Type { get; set; }
            public decimal Rate { get; set; }
        }
    }
}