using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace Restaurant {
    [Serializable]
    public class PaymentZoneController: MarshalByRefObject, ICentralController {
        private const string URI = "Central";
        public event OrderReadyEventHandler OrderReadyEvent;
        public event NewOrderEventHandler NewOrderEvent;
        
        [NonSerialized]
        private static PaymentZoneController instance;
        double total_money;
        Dictionary<string, Product> products;
        [NonSerialized]
        static ConcurrentDictionary<long, Order> orders;    
        
        public PaymentZoneController() {}

        public PaymentZoneController(List<Product> dishes, List<Product> drinks) {
            this.total_money = 0;
        }

        public ICentralController getObj() {
            return instance;
        }

        public override object InitializeLifetimeService() { return (null); }

        public bool InitializeNetwork() {
            try {
                IDictionary dict =  new Hashtable();
                dict["port"] = 8000;
                dict["name"] = "PaymentChannel";
                BinaryServerFormatterSinkProvider ss = new BinaryServerFormatterSinkProvider();
                BinaryClientFormatterSinkProvider cs = new BinaryClientFormatterSinkProvider();
                ss.TypeFilterLevel = TypeFilterLevel.Full;
                ChannelServices.RegisterChannel(new TcpChannel(dict, cs, ss), false);
                RemotingServices.Marshal(this, URI);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to initialize PaymentZoneController network!\n - {0}", e);
                return false;
            }
        }

        public bool TryAndJoinNetwork() {
            return true;
        }

        public bool StartController() {
            Console.WriteLine("Server running, enter <return> to exit");
            Console.ReadLine();
            return true;
        }

        public void OrderReady(long order_id, uint table_n) {

        }

        public void NewOrder(Dictionary<string, uint> products, uint table_n) {
            Console.WriteLine("Got order to table #{0}", table_n);
        }
    }
}