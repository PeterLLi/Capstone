using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone.Models
{
    public class FriendsList
    {
        [Key]
        public int Id { get; set; }

        public string MyId { get; set; }

        public string FriendId { get; set; }
    }
}