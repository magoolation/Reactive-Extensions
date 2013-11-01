using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using RxServer;

namespace RxChatClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Apelido: ");
            string apelido = Console.ReadLine();
            if (string.IsNullOrEmpty(apelido))
                return;
            DuplexChannelFactory<IRxService> dup = null;
            IRxService cli = null;
            ChatCallback call = new ChatCallback();
            dup = new DuplexChannelFactory<IRxService>(call, 
                new NetTcpBinding(), new EndpointAddress(@"net.tcp://localhost:9999/Chat/"));
            dup.Open();
            cli = dup.CreateChannel();
            cli.Register();
            while (true)
            {
                Console.Write("Mensagem:");
                var msg = Console.ReadLine();
                if (string.IsNullOrEmpty(msg))
                    break;
                var message = new Message()
                                  {
                                      From = apelido,
                                      To = "All",
                                      Timestamp = DateTime.Now,
                                      Text = msg
                                  };
                cli.SendMessage(message);
            }
            dup.Close();
        }
    }
}