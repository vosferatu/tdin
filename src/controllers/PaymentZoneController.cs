using Base;
using System;
using System.Threading;
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
    /// <summary>
    /// Responsible for the Central Node of the Restaurant
    /// This controller is the one most directly connected to the 'products' dll
    /// </summary>
    public class PaymentZoneController: MarshalByRefObject, ICentralController {
        public event OrderReadyEventHandler OrderReadyEvent;
        public event NewOrderEventHandler NewDishesOrderEvent;
        public event NewOrderEventHandler NewDrinksOrderEvent;
        
    #region FIELDS
        [NonSerialized]
        double total_money = 0.0;
        [NonSerialized]
        uint selected_table = 0;
        [NonSerialized]
        double order_total = 0.0;
        [NonSerialized]
        PaymentZoneWindow window;
        [NonSerialized]
        Dictionary<string, Product> products;
        [NonSerialized]
        static ConcurrentDictionary<long, Order> orders = 
            new ConcurrentDictionary<long, Order>(2, 10);
        [NonSerialized]
        static ConcurrentDictionary<uint, ConcurrentBag<Order>> table_delivered_orders = 
            new ConcurrentDictionary<uint, ConcurrentBag<Order>>(2, 9);
    #endregion FIELDS

    #region NETWORK_METHODS
        public override object InitializeLifetimeService() { return (null); }

        /// <summary>
        /// Initializes the networking aspects of the controller
        /// </summary>
        /// <returns>Whether the network was initialized or not</returns>
        public bool InitializeNetwork() {
            try {
                IDictionary dict =  new Hashtable();
                dict["port"] = Constants.CENTRAL_PORT;
                dict["name"] = Constants.CENTRAL_CHANNEL_NAME;
                BinaryServerFormatterSinkProvider ss = new BinaryServerFormatterSinkProvider();
                BinaryClientFormatterSinkProvider cs = new BinaryClientFormatterSinkProvider();
                ss.TypeFilterLevel = TypeFilterLevel.Full;
                ChannelServices.RegisterChannel(new TcpChannel(dict, cs, ss), false);
                RemotingServices.Marshal(this, Constants.CENTRAL_URI);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to initialize PaymentZoneController network!\n - {0}", e);
                return false;
            }
        }

        /// <summary>
        /// Starts the controller logic
        /// </summary>
        /// <returns>Retuns on error</returns>
        public bool StartController() {
            this.window = new PaymentZoneWindow(this.OnTableSelect, this.TablePaid);
            Thread thr = new Thread(new ThreadStart(this.window.StartThread));
            try {
                thr.Start();
                Console.WriteLine("Server running, enter <return> to exit");
                Console.ReadLine();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to start PaymentZoneController!\n - {0}", e);
                return false;
            }
        }

    #endregion NETWORK_METHODS

    #region METHODS
        /// <summary>
        /// Creates a new instance of the controller
        /// </summary>
        /// <param name="dishes">List of the dish products</param>
        /// <param name="drinks">List of the drink products</param>
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

        /// <summary>
        /// Callback to be called when the user has selected a table in the UI
        /// </summary>
        /// <param name="table_n">Number of the selected table</param>
        public void OnTableSelect(uint table_n) {
            if (this.selected_table == table_n) {
                this.window.UntoggleButton(table_n);
                this.selected_table = 0;
                this.window.ClearProductsList();
            }
            else {
                this.window.UntoggleButton(this.selected_table);
                this.selected_table = table_n;
                if (table_delivered_orders.ContainsKey(table_n)) {
                    this.window.SetProductsList(this.CreateWidgets(table_n, ref this.order_total), this.order_total);
                }
            }
        }

        /// <summary>
        /// Creates a list of widgets based on the products associated with the orders of a table
        /// </summary>
        /// <param name="table_n">Number of table to view the products</param>
        /// <param name="price">Price of the order will be calculated and stored in this variable</param>
        /// <returns>List of widgets to be shown</returns>
        private List<Gtk.Widget> CreateWidgets(uint table_n, ref double price) {
            Dictionary<Product, uint> prods = this.MergeTableOrders(table_n);
            List<Gtk.Widget> widgets = new List<Gtk.Widget>(prods.Count);
            price = 0.0;
            foreach(KeyValuePair<Product, uint> info in prods) {
                Product prod = info.Key;
                price += prod.price * info.Value;
                widgets.Add(new TableProductEntry(prod.name, prod.price, info.Value));
            }

            return widgets;
        }

        /// <summary>
        /// Merges the list of products of the multiple orders associated with a single table
        /// </summary>
        /// <param name="table_n">Number of table to merge the products</param>
        /// <returns>Dictionary with product and respective amounts</returns>
        private Dictionary<Product, uint> MergeTableOrders(uint table_n) {
            Dictionary<Product, uint> table_orders = new Dictionary<Product, uint>();
            lock(table_delivered_orders) {
                ConcurrentBag<Order> orders = table_delivered_orders[table_n];
                lock (orders) {
                    foreach(Order order in orders) {
                        Dictionary<Product, uint> order_prods = order.GetProducts();
                        foreach(KeyValuePair<Product, uint> order_prod in order_prods) {
                            if (table_orders.ContainsKey(order_prod.Key)) {
                                table_orders[order_prod.Key] = table_orders[order_prod.Key] + order_prod.Value;
                            }
                            else {
                                table_orders.Add(order_prod.Key, order_prod.Value);
                            }

                        }
                    }
                }
            }
            return table_orders;
        }

        /// <summary>
        /// Callback called when a new order is created
        /// Function called by the DiningRoomController
        /// </summary>
        /// <param name="p_infos">Dictionary with products name and amounts</param>
        /// <param name="table_n">Number of table to associate the products</param>
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
            orders.AddOrUpdate(new_order.id, new_order, (k, v) => new_order);
            this.window.NewOrderOnTable(table_n);
            if (new_order.type == OrderTarget.Both && this.NewDishesOrderEvent != null && this.NewDrinksOrderEvent != null) {
                this.NewDishesOrderEvent(new_order.GetOrder(OrderTarget.Kitchen));
                this.NewDrinksOrderEvent(new_order.GetOrder(OrderTarget.Bar));
            }
            else if (new_order.type == OrderTarget.Bar && this.NewDrinksOrderEvent != null) {
                this.NewDrinksOrderEvent(new_order);
            }
            else if (new_order.type == OrderTarget.Kitchen && this.NewDishesOrderEvent != null) {
                this.NewDishesOrderEvent(new_order);
            }
            return;
        }

        /// <summary>
        /// Callback called when an order is ready to be picked up
        /// Function called from the KitchenBarController
        /// </summary>
        /// <param name="order_id">ID of the order that is ready</param>
        /// <param name="table_n">Number of the table associated with the order</param>
        /// <param name="from_kitchen">Whether the order came from the kitchen or not</param>
        public void OrderReady(long order_id, uint table_n, bool from_kitchen) {
            if (orders.ContainsKey(order_id)) {
                Order order = orders[order_id];
                order.SetReady((from_kitchen ? OrderTarget.Kitchen : OrderTarget.Bar));

                if (this.OrderReadyEvent != null && order.IsReady()) {
                    this.OrderReadyEvent(order_id, table_n);
                }
            }
            else {
                Console.WriteLine("Order #{0} does not exist!", order_id);
            }

        }

        /// <summary>
        /// Callback called when the order was delivered to the table
        /// Function called by the DiningRoomController
        /// </summary>
        /// <param name="order_id">ID of the delivered order</param>
        public void OrderDelivered(long order_id) {
            if (orders.ContainsKey(order_id)) {
                Order order = orders[order_id];
                lock (table_delivered_orders) {
                    table_delivered_orders.AddOrUpdate(order.table_n, 
                        new ConcurrentBag<Order> {order}, 
                        (table_n, bag) => {
                            bag.Add(order);
                            return bag;
                    });
                    this.window.OrderDeliveredOnTable(order.table_n);
                }
            }
            else {
                Console.WriteLine("Delivered order #{0} not found!", order_id);
            }
        }

        /// <summary>
        /// Callback called when the table has been paid
        /// Function called by the Payment Zone GUI
        /// </summary>
        public void TablePaid() {
            uint table_n = this.selected_table;
            if (table_delivered_orders.ContainsKey(table_n)) {
                lock(table_delivered_orders) {
                    ConcurrentBag<Order> table_orders = table_delivered_orders[table_n];
                    lock(table_orders) {
                        foreach(Order order in table_orders) {
                            this.total_money += order.TotalPrice();
                            order.Paid();
                        }
                        table_orders.Clear();
                    }
                }
                this.window.SetTotalMoney(this.total_money);
                this.window.ClearProductsList();
            }
            else {
                Console.WriteLine("Table #{0} does not exist!", table_n);
            }
        }
    #endregion METHODS
    }
}