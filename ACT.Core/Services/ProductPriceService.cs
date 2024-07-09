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

        public Dictionary<ProductPriceType, decimal> GetProductRates( int productId, DateTime? date = null )
        {
            List<ProductPrice> productPrices = GetProductPrices( productId, date );

            Dictionary<ProductPriceType, decimal> rates = new Dictionary<ProductPriceType, decimal>();

            foreach ( ProductPrice price in productPrices )
            {
                if ( Enum.IsDefined( typeof( ProductPriceType ), price.Type ) && !rates.ContainsKey( ( ProductPriceType ) price.Type ) )
                {
                    rates[ ( ProductPriceType ) price.Type ] = price.Rate;
                }
            }

            return rates;
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
    }
}