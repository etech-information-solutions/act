using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Services
{
    public class ProductPriceService : BaseService<ProductPrice>, IDisposable
    {
        public ProductPriceService()
        {

        }
        
        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public ProductPrice GetProductDetails(int product, int rate)
        {
            var tt = (from d in context.ProductPrices select d);
            if (tt.Count() > 0)
            {
                var checkRecord = (from s in context.ProductPrices where  s.Status == 1 && s.ProductId == product && 
                                   s.RateUnit == rate select s).FirstOrDefault();

                if (checkRecord != null)
                {
                    return checkRecord;
                }
                else { return null; }
            }
            else
                return null;
        }
    }
}
