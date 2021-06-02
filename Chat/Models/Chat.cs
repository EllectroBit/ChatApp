using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
    public class ChatHub
    {
        public int ID { get; set; }
        public string FirstUser { get; set; }
        public string SecondUser { get; set; }

        public List<Message> Messages{ get; set; }

        public ChatHub()
        {
            Messages = new List<Message>();
        }
    }
}
