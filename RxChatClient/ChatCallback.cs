using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RxServer;

namespace RxChatClient
{
    public class ChatCallback : IRxCallBack
    {
        public void ReceiveMessage(Message message)
        {            
            Console.WriteLine(string.Format("{0} - {1} disse:",
                message.Timestamp.ToString(), message.From));
            Console.WriteLine(string.Format("\t{0}", message.Text));
        }
    }
}