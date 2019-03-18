using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Channels;

namespace Restaurant {
    class CentralNode: MarshalByRefObject {
        private List<Product> dishes {get;}
        private List<Product> drinks {get;}

        private ConcurrentDictionary<Order.State, ConcurrentBag<Order>> orders {get;}
    
        public static void Main(string[] args) {
            List<Product> dishes = ProductReader.readDishes();
            List<Product> drinks = ProductReader.readDrinks();
            if (dishes == null || drinks == null) {
                return;
            }

            CentralNode node = new CentralNode(dishes, drinks);
        }

        CentralNode(List<Product> dishes, List<Product> drinks) {
            this.dishes = dishes;
            this.drinks = drinks;
            this.orders = new ConcurrentDictionary<Order.State, ConcurrentBag<Order>>(4);
            this.orders.AddOrUpdate(Order.State.NotPicked, new ConcurrentBag<Order>(), null);
            this.orders.AddOrUpdate(Order.State.Preparing, new ConcurrentBag<Order>(), null);
            this.orders.AddOrUpdate(Order.State.Ready, new ConcurrentBag<Order>(), null);
            this.orders.AddOrUpdate(Order.State.Paid, new ConcurrentBag<Order>(), null);
        }
    
        
    }

    class CentralServer {
        public static void Main(string[] args) {
            Tcp.TcpChannel ch = new Tcp.TcpChannel(8000);
            ChannelServices.RegisterChannel(ch, false);
            RemotingConfiguration.ApplicationName = "CentralServer";
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(CentralNode),
                "node",
                WellKnownObjectMode.Singleton
            );

            Console.WriteLine("Central Node is up!");
            Console.WriteLine("Press <return> to terminate");
            Console.ReadLine();
        }
    }
}