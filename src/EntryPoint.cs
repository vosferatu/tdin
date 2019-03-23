using Gtk;
using Gdk;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    static class EntryPoint {
        private const string DINING = "DiningRoom";
        private const string KITCHEN = "Kitchen";
        private const string BAR = "Bar";
        private const string CENTRAL = "CentralNode";

        private static List<Order> not_picked = new List<Order>();
        private static List<Order> preparing = new List<Order>();

        public static void Main(string[] args) {
            if (args.Length == 1) {
                Application.Init();
                if (args[0] == DINING) {
                    StartProgram(NewDiningRoom());
                }
                else if (args[0] == KITCHEN || args[0] == BAR) {
                    StartOrders();
                    StartProgram(NewKitchenBar(args[0] == KITCHEN));
                }
                else if (args[0] == CENTRAL) {
                    StartProgram(NewCentralNode());
                }
            }
        }

        private static void StartProgram(IController controller) {
            controller.InitializeNetwork();
            controller.TryAndJoinNetwork();
            controller.StartController();
        }

        private static void StartOrders() {
            Dictionary<Product, uint> order1_p = new Dictionary<Product, uint>();
            Dictionary<Product, uint> order2_p = new Dictionary<Product, uint>();
            order1_p.Add(new Product("Massa", 9.5, 5.4, true), 2);
            order1_p.Add(new Product("CENAS", 9.9, 1.0, true), 1);
            order1_p.Add(new Product("Coke", 1.0, 0.5, false), 3);
            order2_p.Add(new Product("Coke", 1.0, 0.5, false), 3);
            not_picked.Add(Order.NewOrder("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 1, Target.Both, order1_p));
            preparing.Add(Order.NewOrder("MMMCM", 3, Target.Both, order2_p));
        }

        private static KitchenBarController NewKitchenBar(bool is_kitchen) {
            List<Tuple<long, string>> n_picked = new List<Tuple<long, string>>(not_picked.Count);
            List<Tuple<long, string>> prep = new List<Tuple<long, string>>(preparing.Count);
            foreach(Order order in not_picked) {
                n_picked.Add(order.ToTuple());
            }

            foreach(Order order in preparing) {
                prep.Add(order.ToTuple());
            }

            return new KitchenBarController(is_kitchen);
        }

        private static DiningRoomController NewDiningRoom() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new DiningRoomController(dishes, drinks);
        }
    
        private static PaymentZoneController NewCentralNode() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new PaymentZoneController(dishes, drinks);
        }
    }
}
