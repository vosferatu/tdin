using System;
using System.Collections.Generic;

namespace Restaurant {
    public enum Target {Kitchen, Bar, Both};
    public enum State {NotPicked, Preparing, Ready, Paid};
    
    [Serializable]
    public abstract class Order: IEquatable<Order> {
    #region FIELDS
        protected static long curr_id = 0;
        public long id {get; protected set;}
        public uint table_n {get; protected set;}

        public Target type {get; protected set;}
        protected State state;
    #endregion FIELDS

    #region METHODS
        public abstract double TotalTime();
        public abstract double TotalPrice();
        public abstract State GetState();

        public abstract Order GetOrder(Target target);

        protected Order(uint table_n, Target type) {
            this.table_n = table_n;
            this.type = type;
            this.state = State.NotPicked;
        }
    
        public static Order NewOrder(uint table_n, Dictionary<Product, uint> items) {
            Target target = CheckItemsTarget(items);
            if (target == Target.Both) {
                return new CompositeOrder(table_n, target, items);
            }
            return new SimpleOrder(table_n, target, items);
        }

        private static Target CheckItemsTarget(Dictionary<Product, uint> items) {
            uint drinks_n = 0, dishes_n = 0;
            foreach(KeyValuePair<Product, uint> item in items) {
                if (item.Key.type == Product.Type.Dish) {
                    dishes_n++;
                }
                else {
                    drinks_n++;
                }
            }
            if (drinks_n > 0 && dishes_n == 0) return Target.Bar;
            if (drinks_n == 0 && dishes_n > 0) return Target.Kitchen;
            return Target.Both;    
        }
    
        public bool Equals(Order other) {
            return this.id == other.id;
        }
    
        public string ToString() {
            return String.Format("Order #{0}", this.id);
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
        internal CompositeOrder(uint table_n, Target type, Dictionary<Product, uint> items)
            :base (table_n, type) 
        {
            this.id =  Order.curr_id;
            Order.curr_id++;
            Dictionary<Product, uint> drinks = new Dictionary<Product, uint>();
            Dictionary<Product, uint> dishes = new Dictionary<Product, uint>();

            foreach (KeyValuePair<Product, uint> item in items) {
                if (item.Key.type == Product.Type.Dish) {
                    dishes.Add(item.Key, item.Value);
                }
                else {
                    drinks.Add(item.Key, item.Value);
                }
            }

            this.bar_order = new SimpleOrder(this.id, this.table_n, Target.Bar, drinks);
            this.kit_order = new SimpleOrder(this.id, this.table_n, Target.Kitchen, dishes);
        }

        public override Order GetOrder(Target target) {
            Order result = null;
            if (target == Target.Kitchen) result =  this.kit_order;
            else if (target == Target.Bar) result =  this.bar_order;
            return result;
        }
    
        public override double TotalPrice() {
            return this.bar_order.TotalPrice() + this.kit_order.TotalPrice();
        }

        public override double TotalTime() {
            return this.bar_order.TotalTime() + this.kit_order.TotalTime();
        }
    
        public override State GetState() {
            return (State)Math.Min((int)this.bar_order.GetState(), (int)this.kit_order.GetState());
        }
    #endregion METHODS
    }

    [Serializable]
    public class SimpleOrder: Order {
    #region  FIELDS
        Dictionary<Product, uint> items {get;}
    #endregion FIELDS

    #region METHODS
        internal SimpleOrder(long id, uint table_n, Target type, Dictionary<Product, uint> items)
            :base (table_n, type) 
        {
            this.id = id;
        }

        internal SimpleOrder(uint table, Target type, Dictionary<Product, uint> items)
            :base (table, type) 
        {
            this.id = Order.curr_id;
            Order.curr_id++;
        }

        public override Order GetOrder(Target target) {
            return this;
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
    
        public override State GetState() {
            return this.state;
        }
    #endregion METHODS
    }
}