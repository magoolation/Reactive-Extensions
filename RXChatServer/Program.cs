using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using RxServer;

namespace RXChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost svc = new ServiceHost(typeof (RXService));
            Console.WriteLine("Iniciando o serviço");
            svc.Open();
            Console.WriteLine("Serviço iniciado. Pressione ENTER para sair.");
            Console.ReadLine();
            svc.Close();
        }
    }
}