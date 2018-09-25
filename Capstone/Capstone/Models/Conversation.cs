using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone.Models
{
    public class Conversation
    {
        public Conversation()
        {
            status = MessageStatus.Sent;
        }

        public enum MessageStatus
        {
            Sent,
            Delivered
        }

        [Key]
        public int Id { get; set; }
        public string Sender_Id { get; set; }
        public string Receiver_Id { get; set; }
        public string Message { get; set; }
        public MessageStatus status { get; set; }
        public DateTime Created_At { get; set; }
    }
}