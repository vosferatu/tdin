using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    static class EntryPoint {
        static Order order;
        static Product p1;
        static Product p2;
        static Product p3;

        public static void Main(string[] args) {
            StartOrders();
            if (args.Length == 1) {
                Application.Init();
                Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");
                if (args[0] == Constants.START_DINING)  StartProgram(NewDiningRoom());
                else if (args[0] == Constants.START_KITCHEN || args[0] == Constants.START_BAR)  
                    StartProgram(NewKitchenBar(args[0] == Constants.START_KITCHEN));
                else if (args[0] == Constants.START_CENTRAL)  StartProgram(NewCentralNode());
            }
        }

        private static void StartOrders() {
            Dictionary<Product, uint> order1_p = new Dictionary<Product, uint>();
            p1 = new Product("MassaSSSSSSSSSSSSSSSS", 9.5, 5.4, true);
            p2 = new Product("CENAS", 9.9, 1.0, true);
            p3 = new Product("Coke", 1.0, 0.5, false);
            order1_p.Add(p1, 2);
            order1_p.Add(p2, 1);
            order1_p.Add(p3, 3);
            order = Order.NewOrder(1, order1_p);
        }

        private static void StartProgram(IController controller) {
            controller.InitializeNetwork();
            controller.StartController();
            PaymentZoneController cont = (PaymentZoneController)controller;
            cont.NewOrder(order.GetProductsSimplified(), order.table_n);
            cont.OrderReady(1, order.table_n, true);
            cont.OrderDelivered(1);
            Console.WriteLine("Server running, enter <return> to exit");
            Console.ReadLine();
        }

        private static DiningRoomController NewDiningRoom() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new DiningRoomController(dishes, drinks);
        }
    
        private static PaymentZoneController NewCentralNode() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new PaymentZoneController(new List<Product> {p1, p2, p3}, new List<Product>());
        }

        private static KitchenBarController NewKitchenBar(bool is_kitchen) {
            return new KitchenBarController(is_kitchen);
        }
    }
}
