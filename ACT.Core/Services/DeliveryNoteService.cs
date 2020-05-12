using System;
using ACT.Data.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ACT.Core.Enums;
using ACT.Core.Models;

namespace ACT.Core.Services
{
    public class DeliveryNoteService : BaseService<DeliveryNote>, IDisposable
    {
        public DeliveryNoteService()
        {

        }

        public string GetDeliveryNoteNumber()
        {
            string strDelNumber = "";
            string currentDeliveryNote = context.DeliveryNotes
                .OrderByDescending(p => p.Id)
                .Where(r => r.InvoiceNumber != null)
                .Select(r => r.InvoiceNumber)                
                .First().ToString();
            //string queryId = @"Select IDENT_CURRENT('DeliveryNote')";
            //string queryDN = @"SELECT TOP 1
            //                    dl.InvoiceNumber
            //                 FROM
            //                    [dbo].[DeliveryNote] dl";
            //string query = @"SELECT
            //                    REPLACE(STR(dl.Id, 9),' ','0')
            //                 FROM
            //                    [dbo].[DeliveryNote] dl";
            //string currentDeliveryNote = context.Database.SqlQuery<string>(queryDN, new List<object>()).ToString();
            int latestDLNo = Int32.Parse(Regex.Match(currentDeliveryNote, @"\d+").Value);

            //get The current latest number used in deliverynote number and increment and test until we have a number not used, then use that and break
            for (int i = 1; i < 100; i++)
            {
                string numToTest = (latestDLNo + i).ToString().PadLeft(9, '0');
                if (!DeliveryNoteNumberExists(numToTest))
                {
                    strDelNumber = numToTest;
                    break;
                }
            }

            return strDelNumber;
        }

        public bool DeliveryNoteNumberExists(string deliveryNoteNumber)
        {
            return context.DeliveryNotes.Any(c => c.InvoiceNumber == deliveryNoteNumber);
        }

    }
}
