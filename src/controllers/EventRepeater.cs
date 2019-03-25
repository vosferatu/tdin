using System;

namespace Restaurant {
    /// <summary>
    /// Callback class that will be sent around between the nodes
    /// </summary>
    public abstract class EventRepeaterDelegateObject: MarshalByRefObject {
    #region METHODS
        public override object InitializeLifetimeService() {return null;}

        /// <summary>
        /// Function that is called from the DiningRoom when a new order is created
        /// </summary>
        /// <param name="order">Created order</param>
        public void NewOrderCallback(Order order) {
            this.NewOrderCallbackCore(order);
        }

        /// <summary>
        /// Function that is called from the Central Node when a new order is created
        /// </summary>
        /// <param name="order_id">ID of the order</param>
        /// <param name="table_n">Number of table associated with order</param>
        public void OrderReadyCallback(long order_id, uint table_n) {
            this.OrderReadyCallbackCore(order_id, table_n);
        }

        /// <summary>
        /// Function to be overriden by subclasses to provide concrete implementation
        /// </summary>
        /// <param name="order">Order newly created</param>
        protected abstract void NewOrderCallbackCore(Order order);
        /// <summary>
        /// Function to be overriden by subclasses to provide concrete implementation
        /// </summary>
        /// <param name="order_id">ID of the order</param>
        /// <param name="table_n">Number of table associated with order</param>
        protected abstract void OrderReadyCallbackCore(long order_id, uint table_n);
    #endregion METHODS
    }

    public class EventRepeaterDelegate: EventRepeaterDelegateObject {
    #region FIELDS
        OrderReadyEventHandler ready_handler;
        NewOrderEventHandler new_handler;
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Creates a new object with the given handlers
        /// </summary>
        /// <param name="ready_handler">Handler to be called on a ready order</param>
        /// <param name="new_handler">Handler to be called on a new order</param>
        public EventRepeaterDelegate (OrderReadyEventHandler ready_handler, NewOrderEventHandler new_handler) {
            this.ready_handler = ready_handler;
            this.new_handler = new_handler;
        }
        
        /// <summary>
        /// Concrete implementation of the new order callback
        /// </summary>
        /// <param name="order">Newly created order</param>
        protected override void NewOrderCallbackCore(Order order) {
            if (this.new_handler != null) {
                this.new_handler(order);
            }
        }
        
        /// <summary>
        /// Concrete implementation of the order ready callback
        /// </summary>
        /// <param name="order_id">ID of the order</param>
        /// <param name="table_n">Number of table associated with order</param>
        protected override void OrderReadyCallbackCore(long order_id, uint table_n) {
            if (this.ready_handler != null) {
                this.ready_handler(order_id, table_n);
            }
        }
    #endregion METHODS
    }
}