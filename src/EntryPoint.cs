using Gtk;
using Gdk;
using Base;
using Glade;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Restaurant {
    /// <summary>
    /// Main entry point of the program
    /// </summary>
    static class EntryPoint {
        /// <summary>
        /// Main function of the program
        /// </summary>
        /// <param name="args">Console line arguments</param>
        public static void Main(string[] args) {
            if (args.Length == 1) {
                Application.Init();
                Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");
                if (args[0] == Constants.START_DINING)  StartProgram(NewDiningRoom());
                else if (args[0] == Constants.START_KITCHEN || args[0] == Constants.START_BAR)  
                    StartProgram(NewKitchenBar(args[0] == Constants.START_KITCHEN));
                else if (args[0] == Constants.START_CENTRAL)  StartProgram(NewCentralNode());
                else {
                    Console.WriteLine("Wrong usage!");
                    PrintUsage();
                }
            }
            else {
                Console.WriteLine("Wrong usage!");
                PrintUsage();
            }
        }

        /// <summary>
        /// Prints the usage of the program
        /// </summary>
        private static void PrintUsage() {
            Console.WriteLine("Usage:");
            Console.WriteLine("     app.exe <controller>\n");
            Console.WriteLine("Controllers:");
            Console.WriteLine("     CentralNode     Payment zone for the restaurant");
            Console.WriteLine("     DiningRoom      Dining room terminal view");
            Console.WriteLine("     Kitchen         Kitchen room terminal view");
            Console.WriteLine("     Bar             Bar room terminal view\n");
        }

        /// <summary>
        /// Actually starts the program
        /// </summary>
        /// <param name="controller">Starts the program with the given controller</param>
        private static void StartProgram(IController controller) {
            controller.InitializeNetwork();
            controller.StartController();
        }

        /// <summary>
        /// Creates a new dining room controller
        /// </summary>
        /// <returns>Newly created controller</returns>
        private static DiningRoomController NewDiningRoom() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new DiningRoomController(dishes, drinks);
        }

        /// <summary>
        /// Creates a new payment zone controller
        /// </summary>
        /// <returns>Newly created controller</returns>
        private static PaymentZoneController NewCentralNode() {
            List<Product> drinks = ProductReader.ReadDrinks();
            List<Product> dishes = ProductReader.ReadDishes();
            return new PaymentZoneController(dishes, drinks);
        }

        /// <summary>
        /// Creates a new kitchen bar controller
        /// </summary>
        /// <returns>Newly created controller</returns>
        private static KitchenBarController NewKitchenBar(bool is_kitchen) {
            return new KitchenBarController(is_kitchen);
        }
    }
}
