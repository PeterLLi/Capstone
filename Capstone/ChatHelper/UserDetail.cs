using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone.ChatHelper
{
    public class UserDetail
    {
        public string UserID { get; set; }

        public string ConnectionId { get; set; }

        public string UserName { get; set; }
    }
}