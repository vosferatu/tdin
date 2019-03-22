using Gtk;
using System;
using System.Collections.Generic;

namespace Restaurant {
    public class DiningRoomController {
        ProductListWindow window;
        Dictionary<string, uint> order;
        Dictionary<string, Product> products;
        List<Tuple<string, int>> history;

        public DiningRoomController(Dictionary<string, Product> products) {
            this.order = new Dictionary<string, uint>();
            this.history = new List<Tuple<string, int>>();
            this.products = products;
            this.window = new ProductListWindow(this.AddProduct, this.RemProduct, this.SubmitOrder, this.UndoOrder);
        }


        public void AddProduct(string p_name) {
            Console.WriteLine("Adding product '{0}'", p_name);
        }

        public void RemProduct(string p_name) {
            Console.WriteLine("Removing product '{0}'", p_name);
        }

        public void UndoOrder() {
            Console.WriteLine("Resetting order!");
            if (this.history.Count > 0) {
                //TODO: Add or remove product
            }
        }

        public void SubmitOrder() {
            Console.WriteLine("Submitting order!");
        }
    }
}