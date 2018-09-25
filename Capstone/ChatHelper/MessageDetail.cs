using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone.ChatHelper
{
    public class MessageDetail
    {
        public string FromUserID { get; set; }

        public string FromUserName { get; set; }

        public string ToUserID { get; set; }

        public string ToUserName { get; set; }

        public string Message { get; set; }
    }
}