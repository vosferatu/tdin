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
        public bool InitializeNetwork() {
            while (!this.TryRemoteConnection()) {
                Thread.Sleep(Constants.CONNECT_RETRY_DELAY);
                Console.WriteLine("Failed to connect! Retrying...");
            }
            return true;
        }

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
        public DiningRoomController(List<Product> dishes, List<Product> drinks) {
            this.dishes = dishes;
            this.drinks = drinks;
            this.order      = new Dictionary<string, uint>();
            this.history    = new Stack<Tuple<string, int>>();
            this.products   = new Dictionary<string, Product>(dishes.Count + drinks.Count);
            this.ProductsToDict(new List<Product>[] {dishes, drinks}); 
        }

        private List<Tuple<string, double>> ProductsToString(List<Product> products) {
            List<Tuple<string, double>> ret = new List<Tuple<string, double>>(products.Count);
            foreach(Product product in products) {
                ret.Add(new Tuple<string, double>(product.name, product.price));
            }

            return ret;
        }

        private void ProductsToDict(List<Product>[] product_lists) {
            foreach(List<Product> product_list in product_lists) {
                foreach(Product product in product_list) {
                    this.products.Add(product.name, product);
                }
            }
        }

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

        public void SubmitOrder() {
            if (this.order.Count > 0) {
                Console.WriteLine("Submitting order!");
                // TODO Add dinamic table number
                this.central.NewOrder(this.order, 5);
                this.order.Clear();
                this.history.Clear();
                this.window.ResetOrder();
            }
        }

        public void OnOrderReady(long order_id, uint table_n) {
            Console.WriteLine("Order #{0} ready!", order_id);
            // TODO Add behaviour for order ready
        }
    #endregion METHODS
    }
}