using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    static class EntryPoint {
        public static void Main(string[] args) {
            if (args.Length == 1) {
                Application.Init();
                Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");
                if (args[0] == Constants.START_DINING)  StartProgram(NewDiningRoom());
                else if (args[0] == Constants.START_KITCHEN || args[0] == Constants.START_BAR)  
                    StartProgram(NewKitchenBar(args[0] == Constants.START_KITCHEN));
                else if (args[0] == Constants.START_CENTRAL)  StartProgram(NewCentralNode());
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
