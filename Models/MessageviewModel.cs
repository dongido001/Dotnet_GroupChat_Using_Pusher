
using System;

namespace GroupChat.Models
{
    public class MessageViewModel
    {
        public int ID { get; set; }
        public string AddedBy { get; set;  }
        public string message { get; set;  }
        public int GroupId { get; set;  }
        public string SocketId { get; set;  }
    }
}