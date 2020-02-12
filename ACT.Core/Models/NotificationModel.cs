using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACT.Core.Enums;

namespace ACT.Core.Models
{
    public class NotificationModel
    {
        #region Properties

        public string Message { get; set; }

        public NotificationType Type { get; set; }

        #endregion
    }
}