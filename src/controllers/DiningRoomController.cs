using Gtk;
using Base;
using System;
using System.Threading;
using System.Collections;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace Restaurant {
    /// <summary>
    /// Responsible for handling interaction between the user and the DiningRoomWindow
    /// </summary>
    public class DiningRoomController: IController {
    #region FIELDS
        ICentralController central;
        ProductListWindow window;

        Dictionary<string, uint> order;
        Stack<Tuple<string, int>> history;
        Dictionary<string, Product> products;

        List<Product> dishes;
        List<Product> drinks;

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
                props["port"] = Constants.DINING_PORT;  
                props["name"] = Constants.DINING_CHANNEL_NAME;
                BinaryClientFormatterSinkProvider cs = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider ss = new BinaryServerFormatterSinkProvider();
                ss.TypeFilterLevel = TypeFilterLevel.Full;
                ChannelServices.RegisterChannel(new TcpChannel(props, cs, ss), false);
                this.central = (ICentralController)Activator.GetObject(
                    typeof(ICentralController), Constants.FULL_CENTRAL_URI
                );
                EventRepeaterDelegate evnt_del = new EventRepeaterDelegate(this.OnOrderReady, null);
                this.central.OrderReadyEvent += evnt_del.OrderReadyCallback;

                return true;
            }
            catch (Exception e) {
                Console.WriteLine("{0}", e);
            }
            return false;
        }

        /// <summary>
        /// Starts the controller logic
        /// </summary>
        /// <returns>Retuns on error</returns>
        public bool StartController() {
            this.window = new ProductListWindow(this.AddProduct, this.RemProduct, this.SubmitOrder, this.UndoOrder);
            Thread thr = new Thread(new ThreadStart(this.window.StartThread));
            try {
                thr.Start();
                Application.Invoke(delegate {
                    this.window.SetProducts(this.ProductsToString(this.dishes), this.ProductsToString(this.drinks));
                });
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("Failed to start DiningRoomController!\n - {0}", e);
                return false;
            }
        }
    #endregion NETWORK_METHODS

    #region METHODS
        /// <summary>
        /// Creates a new instance of the Dining Room Controller
        /// </summary>
        /// <param name="dishes">List of the dish products</param>
        /// <param name="drinks">List of the drink products</param>
        public DiningRoomController(List<Product> dishes, List<Product> drinks) {
            this.dishes = dishes;
            this.drinks = drinks;
            this.order      = new Dictionary<string, uint>();
            this.history    = new Stack<Tuple<string, int>>();
            this.products   = new Dictionary<string, Product>(dishes.Count + drinks.Count);
            this.ProductsToDict(new List<Product>[] {dishes, drinks}); 
        }

        /// <summary>
        /// Converts a list of products, to its simplified version to be used by the GUI
        /// </summary>
        /// <param name="products">List of products to be converted</param>
        /// <returns>Simplified list of products</returns>
        private List<Tuple<string, double>> ProductsToString(List<Product> products) {
            List<Tuple<string, double>> ret = new List<Tuple<string, double>>(products.Count);
            foreach(Product product in products) {
                ret.Add(new Tuple<string, double>(product.name, product.price));
            }

            return ret;
        }

        /// <summary>
        /// Adds the products to the list of available products in the Dining Room
        /// </summary>
        /// <param name="product_lists">Array of size 2 with dishes in position 0 and drinks in 1</param>
        private void ProductsToDict(List<Product>[] product_lists) {
            foreach(List<Product> product_list in product_lists) {
                foreach(Product product in product_list) {
                    this.products.Add(product.name, product);
                }
            }
        }

        /// <summary>
        /// Adds a product to the list of products in the current order
        /// Function called when clicking the 'Add' button
        /// </summary>
        /// <param name="p_name">Name of product to be added</param>
        /// <param name="add_history">Whether to add this action to the history or not</param>
        public void AddProduct(string p_name, bool add_history) {
            bool is_dish = this.products[p_name].type == ProductType.Dish;
            if (this.order.ContainsKey(p_name)) {
                uint new_amount = this.order[p_name] + 1;
                this.order.Remove(p_name);
                this.order.Add(p_name, new_amount);
                this.window.ChangeAmount(p_name, 1, is_dish);
            }
            else {
                this.order.Add(p_name, 1);
                this.window.AddProduct(p_name, is_dish);
            }

            if (add_history) {
                this.history.Push(new Tuple<string, int>(p_name, 1));
            }
        }

        /// <summary>
        /// Removes a single unit of a product from the list of products in the current order
        /// Function called when clicking the 'Remove' button
        /// </summary>
        /// <param name="p_name">Name of product to be removed</param>
        /// <param name="add_history">Whether to add this product to history or not</param>
        public void RemProduct(string p_name, bool add_history) {
            bool is_dish = this.products[p_name].type == ProductType.Dish;
            if (this.order.ContainsKey(p_name)) {
                uint old_amount = this.order[p_name];
                this.order.Remove(p_name);
                if (old_amount > 1) {
                    this.order.Add(p_name, old_amount-1);
                    this.window.ChangeAmount(p_name, -1, is_dish);
                }
                else {
                    this.window.RemoveProduct(p_name, is_dish);
                }
                if (add_history) {
                    this.history.Push(new Tuple<string, int>(p_name, -1));
                }
            }
        }

        /// <summary>
        /// Undos the previous action the user has made
        /// Function called when user clicks the 'Undo' button
        /// </summary>
        public void UndoOrder() {
            if (this.history.Count > 0) {
                Tuple<string, int> latest = this.history.Pop();
                if (latest.Item2 < 0) {
                    this.AddProduct(latest.Item1,false);
                }
                else {
                    this.RemProduct(latest.Item1, false);
                }
            }
        }

        /// <summary>
        /// Submits the current order list to the central node
        /// Function called when user clicks the 'Submit' button
        /// </summary>
        public void SubmitOrder() {
            if (this.order.Count > 0) {
                this.central.NewOrder(this.order, (uint)this.window.TableNumber.Active + 1);
                this.order.Clear();
                this.history.Clear();
                this.window.ResetOrder();
            }
        }

        /// <summary>
        /// Callback when an order that was previously made is now ready to be picked up by the waiter
        /// Function is called when the PaymentZoneController fires its respective event
        /// </summary>
        /// <param name="order_id">ID of the order that is ready</param>
        /// <param name="table_n">Number of the table associated with the order</param>
        public void OnOrderReady(long order_id, uint table_n) {
            this.window.OrderReady(order_id, this.OnOrderDelivered);
        }

        /// <summary>
        /// Callback when an order is delivered to the table
        /// Function is called when the user clicks the 'Apply' image, on the Order Ready frame
        /// </summary>
        /// <param name="order_id">ID of the order that was delivered</param>
        internal void OnOrderDelivered(long order_id) {
            this.window.FinishOrder(order_id);
            this.central.OrderDelivered(order_id);
        }
    #endregion METHODS
    }
}