using Base;
using System;
using System.Collections.Generic;

namespace Restaurant {
    [Serializable]
    public abstract class Order: IEquatable<Order> {
    #region FIELDS
        protected static long curr_id = 0;
        public long id {get; protected set;}
        public uint table_n {get; protected set;}

        public OrderTarget type {get; protected set;}
        protected OrderState state;
    #endregion FIELDS

    #region METHODS
        public abstract double TotalTime();
        public abstract double TotalPrice();
        public abstract OrderState GetState();

        public abstract Order GetOrder(OrderTarget target);
        public abstract void SetReady(OrderTarget target);
        public abstract bool IsReady();
        public abstract Dictionary<string, uint> GetProductsSimplified();
        public abstract Dictionary<Product, uint> GetProducts();

        protected Order(uint table_n, OrderTarget type) {
            this.table_n = table_n;
            this.type = type;
            this.state = OrderState.NotPicked;
        }
    
        public static Order NewOrder(uint table_n, Dictionary<Product, uint> items) {
            OrderTarget target = CheckItemsTarget(items);
            if (target == OrderTarget.Both) {
                return new CompositeOrder(table_n, target, items);
            }
            return new SimpleOrder(table_n, target, items);
        }

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
    
        public bool Equals(Order other) {
            return this.id == other.id;
        }

        public void Paid() {
            this.state = OrderState.Paid;
        }
    #endregion METHODS
    }

    [Serializable]
    public class CompositeOrder: Order {
    #region FIELDS
        Order bar_order;
        Order kit_order;
    #endregion FIELDS

    #region METHODS
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

        public override Order GetOrder(OrderTarget target) {
            Order result = null;
            if (target == OrderTarget.Kitchen) result =  this.kit_order;
            else if (target == OrderTarget.Bar) result =  this.bar_order;
            return result;
        }
    
        public override Dictionary<string, uint> GetProductsSimplified() {
            Dictionary<string, uint> result = new Dictionary<string, uint>(this.bar_order.GetProductsSimplified());
            foreach(KeyValuePair<string, uint> prod in this.kit_order.GetProductsSimplified()) {
                result.Add(prod.Key, prod.Value);
            }

            return result;
        }

        public override Dictionary<Product, uint> GetProducts() {
            Dictionary<Product, uint> result = new Dictionary<Product, uint>(this.bar_order.GetProducts());
            foreach(KeyValuePair<Product, uint> prod in this.kit_order.GetProducts()) {
                result.Add(prod.Key, prod.Value);
            }
            return result;
        }

        public override double TotalPrice() {
            return this.bar_order.TotalPrice() + this.kit_order.TotalPrice();
        }

        public override double TotalTime() {
            return this.bar_order.TotalTime() + this.kit_order.TotalTime();
        }
    
        public override OrderState GetState() {
            return (OrderState)Math.Min((int)this.bar_order.GetState(), (int)this.kit_order.GetState());
        }
    
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
   
        public override bool IsReady() {
            return this.kit_order.IsReady() && this.bar_order.IsReady();
        }
    #endregion METHODS
    }

    [Serializable]
    public class SimpleOrder: Order {
    #region  FIELDS
        Dictionary<Product, uint> items {get;}
    #endregion FIELDS

    #region METHODS
        internal SimpleOrder(long id, uint table_n, OrderTarget type, Dictionary<Product, uint> items)
            :base (table_n, type) 
        {
            this.id = id;
            this.items = items;
        }

        internal SimpleOrder(uint table, OrderTarget type, Dictionary<Product, uint> items)
            :base (table, type) 
        {
            this.id = Order.curr_id;
            Order.curr_id++;
            this.items = items;
        }

        public override Order GetOrder(OrderTarget target) {
            return this;
        }
    
        public override Dictionary<string, uint> GetProductsSimplified() {
            Dictionary<string, uint> result = new Dictionary<string, uint>(this.items.Count);
            foreach(KeyValuePair<Product, uint> item in this.items) {
                result.Add(item.Key.name, item.Value);
            }

            return result;
        }

        public override Dictionary<Product, uint> GetProducts() {
            return this.items;
        }

        public override double TotalPrice() {
            double price = 0.0;
            foreach (KeyValuePair<Product, uint> entry in this.items) {
                price += entry.Key.price;
            }
            return price;
        }

        public override double TotalTime() {
            double time = 0.0;
            foreach (KeyValuePair<Product, uint> entry in this.items) {
                time += entry.Key.time;
            }
            return time;
        }
    
        public override OrderState GetState() {
            return this.state;
        }
   
        public override void SetReady(OrderTarget target) {
            this.state = OrderState.Ready;
        }

        public override bool IsReady() {
            return this.state == OrderState.Ready;
        }
    #endregion METHODS
    }
}