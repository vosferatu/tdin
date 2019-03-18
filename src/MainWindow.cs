using Gtk;
using Glade;
using System;
using System.Collections.Generic;

namespace Restaurant {
    class MainWindow {
        [Glade.Widget]
        Window root;

        [Glade.Widget]
        Gtk.EventBox starter;

        List<Order> not_picked;
        List<Order> preparing;

        public static void Main(string[] args) {
            MainWindow win = new MainWindow();
            win.StartLoop();
        }

        public MainWindow() {
            Application.Init();
            Glade.XML gxml = new Glade.XML("./assets/windows/starter.glade", "root", null);
            gxml.Autoconnect(this);
            this.not_picked = new List<Order>();
            this.preparing = new List<Order>();
        }

        public void StartLoop() {
            starter.ButtonReleaseEvent += OnStartClicked;
            Application.Run();
        }

        public void OnStartClicked(object o, ButtonReleaseEventArgs e) {
            Console.WriteLine("About to start!");
            this.StartOrders();
            KitchenBarWindow bar_window = this.NewKitchenBar();
            root.HideAll();
            bar_window.Show();
        }

        private void StartOrders() {
            Dictionary<Product, uint> order1_p = new Dictionary<Product, uint>();
            Dictionary<Product, uint> order2_p = new Dictionary<Product, uint>();
            order1_p.Add(new Product("Massa", 9.5, 5.4, true), 2);
            order1_p.Add(new Product("CENAS", 9.9, 1.0, true), 1);
            order1_p.Add(new Product("Coke", 1.0, 0.5, false), 3);

            order2_p.Add(new Product("Coke", 1.0, 0.5, false), 3);
            this.not_picked.Add(Order.NewOrder("SADAD", "Mesa 1", Target.Both, order1_p));
            this.preparing.Add(Order.NewOrder("MMMCM", "Mesa 3", Target.Both, order2_p));
        }

        private KitchenBarWindow NewKitchenBar() {
            List<Tuple<long, string>> n_picked = new List<Tuple<long, string>>(this.not_picked.Count);
            List<Tuple<long, string>> prep = new List<Tuple<long, string>>(this.preparing.Count);
            foreach(Order order in this.not_picked) {
                n_picked.Add(order.ToTuple());
            }

            foreach(Order order in this.preparing) {
                prep.Add(order.ToTuple());
            }

            return new KitchenBarWindow(n_picked, prep);
        }

        public void OnDelete(object o, DeleteEventArgs e) {
            Application.Quit();
        }
    }
}
