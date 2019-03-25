using Base;
using System;
using System.Collections.Generic;

namespace Restaurant {
    [Serializable]
    /// <summary>
    /// Represents a single order of the system. An order can have multiple products contained
    /// </summary>
    public abstract class Order: IEquatable<Order> {
    #region FIELDS
        protected static long curr_id = 0;
        public long id {get; protected set;}
        public uint table_n {get; protected set;}

        public OrderTarget type {get; protected set;}
        protected OrderState state;
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Gets the total time required to prepare the order
        /// </summary>
        /// <returns>Time required to prepare order</returns>
        public abstract double TotalTime();
        /// <summary>
        /// Gets the total price of the order
        /// </summary>
        /// <returns>Total price of the order</returns>
        public abstract double TotalPrice();
        /// <summary>
        /// Gets the current state of the order
        /// </summary>
        /// <returns>Order state</returns>
        public abstract OrderState GetState();
        /// <summary>
        /// Gets the order associated with the given target (Kitchen or Bar)
        /// </summary>
        /// <param name="target">Target of the order</param>
        /// <returns>Corresponding order</returns>
        public abstract Order GetOrder(OrderTarget target);
        /// <summary>
        /// Sets the order's internal state to 'Ready'
        /// </summary>
        /// <param name="target">Target of the order</param>
        public abstract void SetReady(OrderTarget target);
        /// <summary>
        /// Checks if the order is ready
        /// </summary>
        /// <returns>Whether the order is ready or not</returns>
        public abstract bool IsReady();
        /// <summary>
        /// Gets a simplified dictionary with the products, only with name of the product and its amount
        /// </summary>
        /// <returns>Simplified dictionary, without exposing the Product class</returns>
        public abstract Dictionary<string, uint> GetProductsSimplified();
        /// <summary>
        /// Non simplified dictionary with products
        /// </summary>
        /// <returns>Dictionary with Product -> amount</returns>
        public abstract Dictionary<Product, uint> GetProducts();

        /// <summary>
        /// Constructor of an order, to be extended by the subclasses
        /// </summary>
        /// <param name="table_n">Number of table to assign the order</param>
        /// <param name="type">Type of order (Kitchen, Bar, Both)</param>
        protected Order(uint table_n, OrderTarget type) {
            this.table_n = table_n;
            this.type = type;
            this.state = OrderState.NotPicked;
        }

        /// <summary>
        /// Actual constructor used by the program
        /// Checks the given items and creates an instance of the correct order accordingly
        /// </summary>
        /// <param name="table_n">Number of table to assign the order</param>
        /// <param name="items">Items associated with the order</param>
        /// <returns></returns>
        public static Order NewOrder(uint table_n, Dictionary<Product, uint> items) {
            OrderTarget target = CheckItemsTarget(items);
            if (target == OrderTarget.Both) {
                return new CompositeOrder(table_n, target, items);
            }
            return new SimpleOrder(table_n, target, items);
        }

        /// <summary>
        /// Checks the target of the items of the order
        /// </summary>
        /// <param name="items">Items to be checked</param>
        /// <returns>Target of the order</returns>
        private static OrderTarget CheckItemsTarget(Dictionary<Product, uint> items) {
            uint drinks_n = 0, dishes_n = 0;
            foreach(KeyValuePair<Product, uint> item in items) {
                if (item.Key.type == ProductType.Dish) {
                    dishes_n++;
                }
                else {
                    drinks_n++;
                }
            }
            if (drinks_n > 0 && dishes_n == 0) return OrderTarget.Bar;
            if (drinks_n == 0 && dishes_n > 0) return OrderTarget.Kitchen;
            return OrderTarget.Both;    
        }

        /// <summary>
        /// Compares this order to another one
        /// </summary>
        /// <param name="other">Other order to be compared to</param>
        /// <returns>Whether the orders are equal or not</returns>
        public bool Equals(Order other) {
            return this.id == other.id;
        }

        /// <summary>
        /// Sets the order state to paid
        /// </summary>
        public void Paid() {
            this.state = OrderState.Paid;
        }
    #endregion METHODS
    }

    [Serializable]
    /// <summary>
    /// Composite order, that has both Bar and Kitchen products
    /// Has instances of two different orders, each associated with either the Kitchen or the Bar.
    /// </summary>
    public class CompositeOrder: Order {
    #region FIELDS
        Order bar_order;
        Order kit_order;
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Creates a new composite order
        /// </summary>
        /// <param name="table_n">Table number to be associated with the order</param>
        /// <param name="type">Type of the order (Kitchen, Bar, Both)</param>
        /// <param name="items">Items associated with the order</param>
        internal CompositeOrder(uint table_n, OrderTarget type, Dictionary<Product, uint> items)
            :base (table_n, type) 
        {
            this.id =  Order.curr_id;
            Order.curr_id++;
            Dictionary<Product, uint> drinks = new Dictionary<Product, uint>();
            Dictionary<Product, uint> dishes = new Dictionary<Product, uint>();

            foreach (KeyValuePair<Product, uint> item in items) {
                if (item.Key.type == ProductType.Dish) {
                    dishes.Add(item.Key, item.Value);
                }
                else {
                    drinks.Add(item.Key, item.Value);
                }
            }

            this.bar_order = new SimpleOrder(this.id, this.table_n, OrderTarget.Bar, drinks);
            this.kit_order = new SimpleOrder(this.id, this.table_n, OrderTarget.Kitchen, dishes);
        }

        /// <summary>
        /// Gets the order associated with the given target (Kitchen or Bar)
        /// </summary>
        /// <param name="target">Target of the order</param>
        /// <returns>Corresponding order</returns>
        public override Order GetOrder(OrderTarget target) {
            Order result = null;
            if (target == OrderTarget.Kitchen) result =  this.kit_order;
            else if (target == OrderTarget.Bar) result =  this.bar_order;
            return result;
        }

        /// <summary>
        /// Gets a simplified dictionary with the products, only with name of the product and its amount
        /// </summary>
        /// <returns>Simplified dictionary, without exposing the Product class</returns>
        public override Dictionary<string, uint> GetProductsSimplified() {
            Dictionary<string, uint> result = new Dictionary<string, uint>(this.bar_order.GetProductsSimplified());
            foreach(KeyValuePair<string, uint> prod in this.kit_order.GetProductsSimplified()) {
                result.Add(prod.Key, prod.Value);
            }

            return result;
        }

        /// <summary>
        /// Non simplified dictionary with products
        /// </summary>
        /// <returns>Dictionary with Product -> amount</returns>
        public override Dictionary<Product, uint> GetProducts() {
            Dictionary<Product, uint> result = new Dictionary<Product, uint>(this.bar_order.GetProducts());
            foreach(KeyValuePair<Product, uint> prod in this.kit_order.GetProducts()) {
                result.Add(prod.Key, prod.Value);
            }
            return result;
        }

        /// <summary>
        /// Gets the total price of the order
        /// </summary>
        /// <returns>Total price of the order</returns>
        public override double TotalPrice() {
            return this.bar_order.TotalPrice() + this.kit_order.TotalPrice();
        }

        /// <summary>
        /// Gets the total time required to prepare the order
        /// </summary>
        /// <returns>Time required to prepare order</returns>
        public override double TotalTime() {
            return this.bar_order.TotalTime() + this.kit_order.TotalTime();
        }

        /// <summary>
        /// Gets the current state of the order
        /// </summary>
        /// <returns>Order state</returns>
        public override OrderState GetState() {
            return (OrderState)Math.Min((int)this.bar_order.GetState(), (int)this.kit_order.GetState());
        }

        /// <summary>
        /// Sets the order's internal state to 'Ready'
        /// </summary>
        /// <param name="target">Target of the order</param>
        public override void SetReady(OrderTarget target) {
            if (target == OrderTarget.Bar) {
                this.bar_order.SetReady(target);
            }
            else if (target == OrderTarget.Kitchen) {
                this.kit_order.SetReady(target);
            }
            else {
                this.kit_order.SetReady(target);
                this.bar_order.SetReady(target);
            }
        }

        /// <summary>
        /// Checks if the order is ready
        /// </summary>
        /// <returns>Whether the order is ready or not</returns>
        public override bool IsReady() {
            return this.kit_order.IsReady() && this.bar_order.IsReady();
        }
    #endregion METHODS
    }

    [Serializable]
    /// <summary>
    /// Simple order that only has either Bar or Kitchen products
    /// </summary>
    public class SimpleOrder: Order {
    #region  FIELDS
        Dictionary<Product, uint> items {get;}
    #endregion FIELDS

    #region METHODS
        /// <summary>
        /// Creates a simple order
        /// </summary>
        /// <param name="id">ID associated with the order</param>
        /// <param name="table_n">Table number associated with the order</param>
        /// <param name="type">Type of the order (Kitchen or Bar)</param>
        /// <param name="items">Items associated with the order (Either all are Drinks or all are Products)</param>
        internal SimpleOrder(long id, uint table_n, OrderTarget type, Dictionary<Product, uint> items)
            :base (table_n, type) 
        {
            this.id = id;
            this.items = items;
        }

        /// <summary>
        /// Creates a simple order
        /// </summary>
        /// <param name="table">Table number associated with the order</param>
        /// <param name="type">Type of the order (Kitchen  or Bar)</param>
        /// <param name="items">Items associated with the order (Either all are Drinks or all are Products)</param>
        internal SimpleOrder(uint table, OrderTarget type, Dictionary<Product, uint> items)
            :base (table, type) 
        {
            this.id = Order.curr_id;
            Order.curr_id++;
            this.items = items;
        }

        /// <summary>
        /// Gets the order associated with the given target (Kitchen or Bar)
        /// </summary>
        /// <param name="target">Target of the order</param>
        /// <returns>Corresponding order</returns>
        public override Order GetOrder(OrderTarget target) {
            return this;
        }

        /// <summary>
        /// Gets a simplified dictionary with the products, only with name of the product and its amount
        /// </summary>
        /// <returns>Simplified dictionary, without exposing the Product class</returns>
        public override Dictionary<string, uint> GetProductsSimplified() {
            Dictionary<string, uint> result = new Dictionary<string, uint>(this.items.Count);
            foreach(KeyValuePair<Product, uint> item in this.items) {
                result.Add(item.Key.name, item.Value);
            }

            return result;
        }

        /// <summary>
        /// Non simplified dictionary with products
        /// </summary>
        /// <returns>Dictionary with Product -> amount</returns>
        public override Dictionary<Product, uint> GetProducts() {
            return this.items;
        }

        /// <summary>
        /// Gets the total price of the order
        /// </summary>
        /// <returns>Total price of the order</returns>
        public override double TotalPrice() {
            double price = 0.0;
            foreach (KeyValuePair<Product, uint> entry in this.items) {
                price += entry.Key.price * entry.Value;
            }
            return price;
        }

        /// <summary>
        /// Gets the total time required to prepare the order
        /// </summary>
        /// <returns>Time required to prepare order</returns>
        public override double TotalTime() {
            double time = 0.0;
            foreach (KeyValuePair<Product, uint> entry in this.items) {
                time += entry.Key.time;
            }
            return time;
        }

        /// <summary>
        /// Gets the current state of the order
        /// </summary>
        /// <returns>Order state</returns>
        public override OrderState GetState() {
            return this.state;
        }

        /// <summary>
        /// Sets the order's internal state to 'Ready'
        /// </summary>
        /// <param name="target">Target of the order</param>
        public override void SetReady(OrderTarget target) {
            this.state = OrderState.Ready;
        }

        /// <summary>
        /// Checks if the order is ready
        /// </summary>
        /// <returns>Whether the order is ready or not</returns>
        public override bool IsReady() {
            return this.state == OrderState.Ready;
        }
    #endregion METHODS
    }
}