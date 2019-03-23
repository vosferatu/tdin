using Gtk;
using Glade;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;


namespace Restaurant {
    public class KitchenBarController: IController {
        #region FIELDS
        private const string REMOTE_URI = "tcp://localhost:8000/Central";
        
        ICentralController central;
        OrderDetailWindow detail_window;
        OrderListWindow list_window;

        List<Order> not_picked;
        List<Order> preparing;
        bool is_kitchen;
        #endregion FIELDS

        #region NETWORK_METHODS
        public bool InitializeNetwork() {
            while (!this.TryRemoteConnection()) {
                Thread.Sleep(1990);
                Console.WriteLine("Failed to connect! Retrying...");
            }
            return true;
        }

        public bool TryRemoteConnection() {
            try {
                Hashtable props = new Hashtable();
                props["port"] = 0;  
                props["name"] = "KitchenBarChannel";
                BinaryClientFormatterSinkProvider cs = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider ss = new BinaryServerFormatterSinkProvider();
                ss.TypeFilterLevel = TypeFilterLevel.Full;
                ChannelServices.RegisterChannel(new TcpChannel(props, cs, ss), false);
                this.central = (ICentralController)Activator.GetObject(
                    typeof(ICentralController), REMOTE_URI
                );

                EventRepeaterDelegate evnt_del = new EventRepeaterDelegate(null, this.OnNewOrder);
                if (this.is_kitchen) {
                    this.central.NewDishesOrderEvent += evnt_del.NewOrderCallback;
                }
                else {
                    this.central.NewDrinksOrderEvent += evnt_del.NewOrderCallback;
                }
                return true;
            }
            catch (Exception e) {}
            return false;
        }

        public bool StartController() {
            this.list_window = new OrderListWindow(this.ViewOrderDetails, this.MoveOrder, this.FinishOrder);
            Thread thr = new Thread(new ThreadStart(this.list_window.StartThread));
            try {
                thr.Start();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to start KitchenBarController!\n - {0}", e);
                return false;
            }
        }
        #endregion NETWORK_METHODS

        #region METHODS
        public KitchenBarController(bool is_kitchen) {
            this.is_kitchen = is_kitchen;
            this.not_picked = new List<Order>();
            this.preparing = new List<Order>();
        }

        public void OnNewOrder(Order order) {
            this.not_picked.Add(order);
            Application.Invoke(delegate {
                this.list_window.AddOrder(new Tuple<long, string>(order.id, order.ToString()));
            });
        }

        public void ViewOrderDetails(long order_id) {
            Console.WriteLine("Cliked on order #{0}, ViewOrderDetails", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null) {
                this.detail_window = new OrderDetailWindow(this.GoBack, order.id, order.ToString(), order.table_n);
                this.list_window.root.HideAll();
                this.detail_window.root.ShowAll();
            }
        }

        public void MoveOrder(long order_id) {
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.PickOrder(order_id)) {
                this.preparing.Add(order);
                this.not_picked.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
            }
        }

        public void FinishOrder(long order_id) {
            Console.WriteLine("Cliked on order #{0}, FinishOrder", order_id);
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.RemoveOrder(order_id)) {
                this.preparing.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
                this.central.OrderReady(order_id, order.table_n);
            }
        }

        public void GoBack(object o, EventArgs args) {
            this.detail_window.root.HideAll();
            this.list_window.root.ShowAll();
        }

        private Order FindOrder(long order_id) {
            foreach(Order order in this.not_picked) {
                if (order.id == order_id) {
                    return order;
                }
            }
            foreach(Order order in this.preparing) {
                if (order.id == order_id) {
                    return order;
                }
            }

            return null;
        }
        #endregion METHODS
    }
}