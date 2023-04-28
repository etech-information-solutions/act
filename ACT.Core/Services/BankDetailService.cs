using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class BankDetailService : BaseService<BankDetail>, IDisposable
    {
        public BankDetailService()
        {

        }

        /// </summary>      
        /// <param name="model">model of the user to be fetched</param>
        /// <returns></returns>
        public BankDetail CheckAccount()
        {
            var tt = (from d in context.BankDetails select d);
            if (tt.Count() > 0)
            {
                BankDetail heckRecord = (from s in context.BankDetails select s).FirstOrDefault();

                if (heckRecord != null)
                {
                    return heckRecord;
                }
                else return null;

            }
            else
                return null;


        }
    }
}
