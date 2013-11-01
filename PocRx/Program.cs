    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PocRx
{
    internal class Program
    {

        private static ObservableCollection<string> lista = new ObservableCollection<string>();
        
        private static void Main(string[] args)
        {
            IObservable<int> source = Observable.Range(0, 10);

            var filter = from s in source
                         where s%2 == 0
                         select s;

            var obs = Observable.FromEventPattern(
                (EventHandler<NotifyCollectionChangedEventArgs> ev) => new NotifyCollectionChangedEventHandler(ev),
                ev => lista.CollectionChanged += ev,
                ev => lista.CollectionChanged -= ev);

            var query = from o in obs
                        where o.EventArgs.Action == NotifyCollectionChangedAction.Add
                        from s in o.EventArgs.NewItems.Cast<string>().ToObservable()
                        select s;

            var buffer = query.Buffer(10);

            IDisposable subscriptionList =
                buffer
                    .Subscribe(
                        x => { 
            foreach (var s in x.Reverse())
            {
                ImprimeAzul(s);
            }
        }

    ,
                    ex => Console.WriteLine(ex.Message),
                    () => Console.WriteLine("Sem mensagens")
                    );


            IDisposable subscriptionList1 =
                buffer
                    .Subscribe(
                        x =>
                        {
                            foreach (var s in x.Reverse())
                            {
ImprimeVermelho(s);
                            }
                        }

    ,
                    ex => Console.WriteLine(ex.Message),
                    () => Console.WriteLine("Sem mensagens")
                    );

            
            IDisposable subscription = filter.Subscribe(
                x => Console.WriteLine(x),
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("End"));

            while(true)
            {
                var mensagem = Console.ReadLine();
                if (string.IsNullOrEmpty(mensagem))
                    break;
                lista.Add(mensagem);
            }

            Console.ReadLine();
            subscription.Dispose();
            subscriptionList.Dispose();            
            subscriptionList1.Dispose();
        }

        private static void ImprimeVermelho(object o)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.WriteLine(o);
            Console.ForegroundColor = oldColor;
        }

        private static void ImprimeAzul(object o)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = System.ConsoleColor.Blue;
            Console.WriteLine(o);
            Console.ForegroundColor = oldColor;
        }
    }
}