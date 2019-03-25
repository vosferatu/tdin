using System;
using System.Collections.Generic;

namespace Restaurant {
    /// <summary>
    /// Represents a callback when the order is ready
    /// </summary>
    /// <param name="order_id">Number of the order that is ready</param>
    /// <param name="table_n">Table number associated with that order</param>
    public delegate void OrderReadyEventHandler(long order_id, uint table_n);
    /// <summary>
    /// Callback when a new order is created
    /// </summary>
    /// <param name="order">Newly created order</param>
    public delegate void NewOrderEventHandler(Order order);

    /// <summary>
    /// Interface associated with a generic controller of a GUI
    /// </summary>
    public interface IController {
        /// <summary>
        /// Initializes the networking aspects of the controller
        /// </summary>
        /// <returns>Whether the network was initialized or not</returns>
        bool InitializeNetwork();
        /// <summary>
        /// Starts the controller logic
        /// </summary>
        /// <returns>Retuns on error</returns>
        bool StartController();
    }

    /// <summary>
    /// Interface of the central node controller (Payment Zone Controller)
    /// </summary>
    public interface ICentralController: IController {
        /// <summary>
        /// Event that is called when an order is ready
        /// </summary>
        event OrderReadyEventHandler OrderReadyEvent;
        /// <summary>
        /// Event that is called when a new order only with dishes is created
        /// </summary>
        event NewOrderEventHandler NewDishesOrderEvent;
        /// <summary>
        /// Event that is called when a new order only with drinks is created
        /// </summary>
        event NewOrderEventHandler NewDrinksOrderEvent;
        
        /// <summary>
        /// Function called when an order is ready
        /// </summary>
        /// <param name="order_id">ID of the order that is ready</param>
        /// <param name="table_n">Table number associated with that order</param>
        /// <param name="from_kitchen">Whether the order came from the kitchen or not</param>
        void OrderReady(long order_id, uint table_n, bool from_kitchen);
        /// <summary>
        /// Function called when an order is created
        /// </summary>
        /// <param name="products">Dictionary with products name and amount of the created order</param>
        /// <param name="table_n">Table number associated with the order</param>
        void NewOrder(Dictionary<string, uint> products, uint table_n);
        /// <summary>
        /// Function called when the order is delivered to the table
        /// </summary>
        /// <param name="order_id">ID of the delivered order</param>
        void OrderDelivered(long order_id);
    }
}