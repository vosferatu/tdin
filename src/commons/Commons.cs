using System;

namespace Base {
    public enum OrderTarget {Kitchen, Bar, Both};
    public enum OrderState {NotPicked, Preparing, Ready, Paid};
    public enum ProductType {Dish, Drink};
    public delegate void SimpleFunction();

    public static class Constants {
    #region CENTRAL
        public const string CENTRAL_CHANNEL_NAME = "PaymentChannel";
        public const uint   CENTRAL_PORT = 8000;
        public const string CENTRAL_HOST = "localhost"; 
        public const string CENTRAL_URI = "Central";
        public static string FULL_CENTRAL_URI = String.Format("tcp://{0}:{1}/{2}", CENTRAL_HOST, CENTRAL_PORT, CENTRAL_URI);
    #endregion CENTRAL

    #region KITCHENBAR
        public const string KITCHENBAR_CHANNEL_NAME = "KitchenBarChannel";
        public const uint KITCHENBAR_PORT = 0; // Select first available port
    #endregion KITCHENBAR

    #region DINING
        public const string DINING_CHANNEL_NAME = "DiningRoomChannel";
        public const uint DINING_PORT = 0; // Select first available
    #endregion DINING

    #region MAIN
        public const string START_DINING = "DiningRoom";
        public const string START_KITCHEN = "Kitchen";
        public const string START_BAR = "Bar";
        public const string START_CENTRAL = "CentralNode";
    #endregion MAIN

        public const int CONNECT_RETRY_DELAY = 1990;
        public const string ASSETS_DIR = "./assets/";
    }   
}