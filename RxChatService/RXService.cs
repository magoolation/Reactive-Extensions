using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RxServer
{    
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single
        )]
    public class RXService : IRxService, IDisposable
    {
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
       private Dictionary<IRxCallBack, IDisposable> subscriptions = new Dictionary<IRxCallBack, IDisposable>();         
                
        private void SendErrorToClient(Exception ex)
        {
            var message = new Message()
                              {
                                  From = "System",
                                  To = "All",
                                  Timestamp = DateTime.Now,
                                  Text = ex.Message
                              };
            messages.Add(message);
        }

        public void SendFinishToClient()
        {
            var message = new Message()
                              {
                                  From = "System",
                                  To = "All",
                                  Timestamp = DateTime.Now,
                                  Text = "Chat finalizado"
                              };
            messages.Add(message);
        }

        public void Register()
        {            
            Console.WriteLine("Registering client");            
            Console.WriteLine(OperationContext.Current.Channel.RemoteAddress);
            var channel = OperationContext.Current.GetCallbackChannel<IRxCallBack>();

            var ep = Observable.FromEventPattern(
                (EventHandler<NotifyCollectionChangedEventArgs> ev) => new NotifyCollectionChangedEventHandler(ev),
                ev => messages.CollectionChanged += ev,
                ev => messages.CollectionChanged -= ev);

            var qr = from e in ep
                     where e.EventArgs.Action == NotifyCollectionChangedAction.Add
                     from m in e.EventArgs.NewItems.Cast<Message>()
                     select m;

            subscriptions.Add(channel, qr.Subscribe(
                m =>
                    {
                        try
                        {
                            ICommunicationObject connection = channel as ICommunicationObject;
                                if (connection != null)
                                    if (connection.State == CommunicationState.Opened)
                                        channel.ReceiveMessage(m);
                        }
                        catch
                        {
                            subscriptions[channel].Dispose();
                        }
                    },
                ex => SendErrorToClient(ex),
                () => SendFinishToClient()
                ));

            messages.Add(new Message()
                                    {
                                        To = "All",
                                        From = "System",
                                        Timestamp = DateTime.Now,
                                        Text = "Entrou na sala"
                                    });
        }

        public void SendMessage(Message message)
        {
            Console.WriteLine("Message received:");
            Console.WriteLine(string.Format("{0}: {1}",
                message.From, message.Text));
            messages.Add(message);           ;
        }
        
        public void Dispose()
        {
            foreach(var key in subscriptions.Keys)
                subscriptions[key].Dispose();
                
        }
    }
}