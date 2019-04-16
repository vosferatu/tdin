using Gtk;
using Base;
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
    /// <summary>
    /// Responsible for managing the KichenBar GUI
    /// </summary>
    public class KitchenBarController: IController {
    #region FIELDS
        ICentralController central;
        string prev_title;
        Gtk.Widget[] prev_window;
        OrderDetailWindow detail_window;
        OrderListWindow list_window;

        List<Order> not_picked;
        List<Order> preparing;
        bool is_kitchen;
    #endregion FIELDS

    #region NETWORK_METHODS
        /// <summary>
        /// Initializes the networking aspects of the controller
        /// </summary>
        /// <returns>Whether the network was initialized or not</returns>
        public bool InitializeNetwork() {
            while (!this.TryRemoteConnection()) {
                Thread.Sleep(Constants.CONNECT_RETRY_DELAY);
                Console.WriteLine("Failed to connect! Retrying...");
            }
            return true;
        }

        /// <summary>
        /// Attempts to connect to the central node
        /// </summary>
        /// <returns>Whether the connection was sucessfull or not</returns>
        private bool TryRemoteConnection() {
            try {
                Hashtable props = new Hashtable();
                props["port"] = Constants.KITCHENBAR_PORT;  
                props["name"] = Constants.KITCHENBAR_CHANNEL_NAME;
                BinaryClientFormatterSinkProvider cs = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider ss = new BinaryServerFormatterSinkProvider();
                ss.TypeFilterLevel = TypeFilterLevel.Full;
                ChannelServices.RegisterChannel(new TcpChannel(props, cs, ss), false);
                this.central = (ICentralController)Activator.GetObject(
                    typeof(ICentralController), Constants.FULL_CENTRAL_URI
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
            catch (Exception) {}
            return false;
        }

        /// <summary>
        /// Starts the controller logic
        /// </summary>
        /// <returns>Retuns on error</returns>
        public bool StartController() {
            this.list_window = new OrderListWindow(this.ViewOrderDetails, this.MoveOrder, this.FinishOrder);
            Thread thr = new Thread(new ThreadStart(this.list_window.StartThread));
            try {
                thr.Start();
                while (this.list_window.root == null) {}
                this.list_window.root.Title = (this.is_kitchen ? "Kitchen Orders" : "Bar Orders");
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to start KitchenBarController!\n - {0}", e);
                return false;
            }
        }
    #endregion NETWORK_METHODS

    #region METHODS
        /// <summary>
        /// Creates a new instance of the KitchenBarController
        /// </summary>
        /// <param name="is_kitchen">Whether this controller is for the Kitchen or not</param>
        public KitchenBarController(bool is_kitchen) {
            this.is_kitchen = is_kitchen;
            this.not_picked = new List<Order>();
            this.preparing = new List<Order>();
        }
        
        /// <summary>
        /// Callback called when a new order is created
        /// Function called when clicking on the button 'Submit'
        /// </summary>
        /// <param name="order">Newly constructed order</param>
        public void OnNewOrder(Order order) {
            this.not_picked.Add(order);
            Application.Invoke(delegate {
                this.list_window.AddOrder(order.id);
            });
        }

        /// <summary>
        /// Callback called when user clicks order and needs to view its details
        /// Function called when clicking the label of a order
        /// </summary>
        /// <param name="order_id">ID of the clicked order</param>
        public void ViewOrderDetails(long order_id) {
            Order order = this.FindOrder(order_id);
            if (order != null) {
                this.detail_window = new OrderDetailWindow(this.GoBack, order.GetProductsSimplified(), order.table_n);
                this.prev_window = this.list_window.root.Children;
                this.prev_title = this.list_window.root.Title;
                this.list_window.root.Title = String.Format("Order #{0} Details", order.id);
                foreach(Gtk.Widget widget in this.list_window.root.Children) 
                    this.list_window.root.Remove(widget);
                
                foreach(Gtk.Widget widget in this.detail_window.root.Children) {
                    widget.Unparent();
                    this.list_window.root.Add(widget);
                }
                this.list_window.root.ShowAll();
            }
        }

        /// <summary>
        /// Moves an order from the 'Not Picked' to the 'Preparing' state
        /// Function called when clicking the 'Forward' image, next to the label in the Not Picked frame
        /// </summary>
        /// <param name="order_id">ID of the moved order</param>
        public void MoveOrder(long order_id) {
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.PickOrder(order_id)) {
                this.preparing.Add(order);
                this.not_picked.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
            }
        }

        /// <summary>
        /// Removes the order from the GUI, signals that it is ready to be picked
        /// Function called when clicking the 'Apply' image, next to the label in the Preparing frame
        /// </summary>
        /// <param name="order_id">ID of the finished order</param>
        public void FinishOrder(long order_id) {
            Order order = this.FindOrder(order_id);
            if (order != null && this.list_window.RemoveOrder(order_id)) {
                this.preparing.Remove(order);
                this.list_window.root.ShowAll();
                this.list_window.root.QueueDraw();
                this.central.OrderReady(order_id, order.table_n, this.is_kitchen);
            }
        }

        /// <summary>
        /// Returns to the default KitchenBarWindow interface
        /// </summary>
        /// <param name="o">Object that called this function</param>
        /// <param name="args">Arguments of the event</param>
        public void GoBack(object o, EventArgs args) {
            foreach(Gtk.Widget widget in this.list_window.root.Children) 
                widget.Destroy();
        
            foreach(Gtk.Widget widget in this.prev_window) {
                this.list_window.root.Add(widget);
                widget.ShowAll();
            }
            this.list_window.root.Title = this.prev_title;
        }

        /// <summary>
        /// Finds the specified order in the list of orders
        /// </summary>
        /// <param name="order_id">ID of the order to be found</param>
        /// <returns>Specified order or null on not found</returns>
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