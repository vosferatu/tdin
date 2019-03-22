using Gtk;
using Gdk;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    class MainWindow {
        
        [Glade.Widget]
        Gtk.Window root;
        [Glade.Widget]
        Gtk.EventBox starter;

        List<Order> not_picked;
        List<Order> preparing;

        Gtk.StatusIcon icons;

        public static void Main(string[] args) {
            MainWindow win = new MainWindow();
        }

        public MainWindow() {
            DiningRoomController cont = this.NewDiningRoom();
        }

        public void OnStartClicked(object o, ButtonReleaseEventArgs e) {
            Console.WriteLine("About to start!");
            this.StartOrders();
            root.HideAll();
            DiningRoomController bar_window = this.NewDiningRoom();
        }

        private void StartOrders() {
            Dictionary<Product, uint> order1_p = new Dictionary<Product, uint>();
            Dictionary<Product, uint> order2_p = new Dictionary<Product, uint>();
            order1_p.Add(new Product("Massa", 9.5, 5.4, true), 2);
            order1_p.Add(new Product("CENAS", 9.9, 1.0, true), 1);
            order1_p.Add(new Product("Coke", 1.0, 0.5, false), 3);

            order2_p.Add(new Product("Coke", 1.0, 0.5, false), 3);
            this.not_picked.Add(Order.NewOrder("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 1, Target.Both, order1_p));
            this.preparing.Add(Order.NewOrder("MMMCM", 3, Target.Both, order2_p));
        }

        private KitchenBarController NewKitchenBar() {
            List<Tuple<long, string>> n_picked = new List<Tuple<long, string>>(this.not_picked.Count);
            List<Tuple<long, string>> prep = new List<Tuple<long, string>>(this.preparing.Count);
            foreach(Order order in this.not_picked) {
                n_picked.Add(order.ToTuple());
            }

            foreach(Order order in this.preparing) {
                prep.Add(order.ToTuple());
            }

            return new KitchenBarController();
        }

        private DiningRoomController NewDiningRoom() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            DiningRoomController controller = new DiningRoomController(dishes, drinks);
            return controller;
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}
