using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
    public class Message
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }

        public ChatHub ChatHub { get; set; }
        public int ChatHubID { get; set; }
        public int? SenderID { get; set; }
        public User Sender { get; set; }
    }
}
