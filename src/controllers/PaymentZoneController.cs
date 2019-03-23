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
    #region FIELDS
        private const string URI = "Central";
        public event OrderReadyEventHandler OrderReadyEvent;
        public event NewOrderEventHandler NewDishesOrderEvent;
        public event NewOrderEventHandler NewDrinksOrderEvent;
        
        double total_money;
        Dictionary<string, Product> products;
        [NonSerialized]
        static ConcurrentDictionary<long, Order> orders;    
    #endregion FIELDS

    #region NETWORK_METHODS
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


        public bool StartController() {
            Console.WriteLine("Server running, enter <return> to exit");
            Console.ReadLine();
            return true;
        }

    #endregion NETWORK_METHODS

    #region METHODS
        public PaymentZoneController(List<Product> dishes, List<Product> drinks) {
            this.total_money = 0;
            this.products = new Dictionary<string, Product>(dishes.Count + drinks.Count);
            foreach(Product dish in dishes) {
                this.products.Add(dish.name, dish);
            }
            foreach(Product drink in drinks) {
                this.products.Add(drink.name, drink);
            }
        }

        public void OrderReady(long order_id, uint table_n) {
            Console.WriteLine("Order #{0} ready!", order_id);

            if (this.OrderReadyEvent != null) {
                this.OrderReadyEvent(order_id, table_n);
            }
        }

        public void NewOrder(Dictionary<string, uint> p_infos, uint table_n) {
            Dictionary<Product, uint> p = new Dictionary<Product, uint>(p_infos.Count);
            foreach(KeyValuePair<string, uint> p_info in p_infos) {
                if (this.products.ContainsKey(p_info.Key)) {
                    p.Add(this.products[p_info.Key], p_info.Value);
                }
                else {
                    Console.WriteLine("Product '{0}' not found! Aborting order...", p_info.Key);
                    return;
                }
            }

            Order new_order = Order.NewOrder(table_n, p);
            if (new_order.type == Target.Both && this.NewDishesOrderEvent != null && this.NewDrinksOrderEvent != null) {
                this.NewDishesOrderEvent(new_order.GetOrder(Target.Kitchen));
                this.NewDrinksOrderEvent(new_order.GetOrder(Target.Bar));
            }
            else if (new_order.type == Target.Bar && this.NewDrinksOrderEvent != null) {
                this.NewDrinksOrderEvent(new_order);
            }
            else if (new_order.type == Target.Kitchen && this.NewDishesOrderEvent != null) {
                this.NewDishesOrderEvent(new_order);
            }
            return;
        }
    #endregion METHODS
    }
}