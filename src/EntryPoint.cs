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
                if (args[0] == DINING)  StartProgram(NewDiningRoom());
                else if (args[0] == KITCHEN || args[0] == BAR)  StartProgram(NewKitchenBar(args[0] == KITCHEN));
                else if (args[0] == CENTRAL)  StartProgram(NewCentralNode());
            }
        }

        private static void StartProgram(IController controller) {
            controller.InitializeNetwork();
            controller.StartController();
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

        private static KitchenBarController NewKitchenBar(bool is_kitchen) {
            return new KitchenBarController(is_kitchen);
        }
    }
}
