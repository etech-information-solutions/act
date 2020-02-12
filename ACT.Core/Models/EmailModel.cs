using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ACT.Core.Models
{
    public class EmailModel : IDisposable
    {
        private bool disposing = false;

        #region Model Properties

        public string From { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        public List<string> Recipients { get; set; }

        public List<Attachment> Attachments { get; set; }

        #endregion

        public EmailModel()
        {
            this.Attachments = new List<Attachment>();
        }

        public void Dispose()
        {
            this.disposing = true;
        }
    }
}