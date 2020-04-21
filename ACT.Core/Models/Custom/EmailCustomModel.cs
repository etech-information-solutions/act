
using System;
using System.Collections.Generic;

namespace ACT.Core.Models.Custom
{
    public partial class EmailCustomModel
    {
        public EmailCustomModel()
        {
            this.Attachments = new List<AttachmentCustomModel>();
        }
        public int MessageNumber { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public System.DateTime DateSent { get; set; }
        public List<AttachmentCustomModel> Attachments { get; set; }
    }
    public partial class AttachmentCustomModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

    }
