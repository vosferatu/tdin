using Gtk;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    public class DiningRoomController {
        ProductListWindow window;
        Dictionary<string, uint> order;
        Dictionary<string, Product> products;
        Stack<Tuple<string, int>> history;

        List<Product> dishes;
        List<Product> drinks;

        public DiningRoomController(List<Product> dishes, List<Product> drinks) {
            this.order      = new Dictionary<string, uint>();
            this.history    = new Stack<Tuple<string, int>>();
            this.products   = new Dictionary<string, Product>(dishes.Count + drinks.Count);
            this.ProductsToDict(new List<Product>[] {dishes, drinks});
            this.window     = new ProductListWindow(this.AddProduct, this.RemProduct, this.SubmitOrder, this.UndoOrder);
            Thread thr = new Thread(new ThreadStart(this.window.StartThread));
            thr.Start();
            Thread.Sleep(100);
            Application.Invoke(delegate {
                this.window.SetProducts(this.ProductsToString(dishes), this.ProductsToString(drinks));
            });
        }

        private void StartThread() {
            this.window.StartThread();
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
            bool is_dish = this.products[p_name].type == Product.Type.Dish;
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
            bool is_dish = this.products[p_name].type == Product.Type.Dish;
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
                // TODO: Send order to the network
            }
        }
    }
}