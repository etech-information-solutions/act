using ACT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACT.Core.Services
{
  public  class BillingInvoiceLineItemService : BaseService<BillingInvoiceLineItem>, IDisposable
    {
        public BillingInvoiceLineItemService()
        {

        }

        /// <summary>
        /// Gets a billing invoice the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override BillingInvoiceLineItem GetById(int id)
        {
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.ProxyCreationEnabled = true;

            return base.GetById(id);
        }

        public List<BillingInvoiceLineItem> ListByBillingInvoice(int billingId)
        {
            return context.BillingInvoiceLineItems.Where(bi => bi.BillingInvoiceId == billingId).ToList();
        }

        /// <summary>
        /// Gets a Billing Item List by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BillingInvoiceLineItem> BillingList(int id)
        {
            return context.Database.SqlQuery<BillingInvoiceLineItem>($"SELECT * FROM [dbo].[BillingInvoiceLineItem] bi WHERE bi.[ID]='{id}';").ToList();
        }
    }
}
