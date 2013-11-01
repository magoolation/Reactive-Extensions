using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RxServer
{

    [DataContract]
    public class Message
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
    
    public interface IRxCallBack
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Message message);
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(
        SessionMode = SessionMode.Required,
        CallbackContract = typeof (IRxCallBack))]
    public interface IRxService
    {
        [OperationContract(IsOneWay = true)]
        void Register();

        [OperationContract(IsOneWay = true)]
        void SendMessage(Message message);
    }
}