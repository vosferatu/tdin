using System;
using System.Collections.Generic;

namespace Restaurant {
    public enum Target {Kitchen, Bar, Both};
    public enum State {NotPicked, Preparing, Ready, Paid};
    
    public abstract class Order{
        protected static long curr_id = 0;
        protected long id {get; set;}
        public string desc {get;}
        protected string table {get;}

        protected Target type {get;}
        protected State state;

        public abstract double TotalTime();
        public abstract double TotalPrice();
        public abstract State GetState();

        public abstract Dictionary<Product, uint> GetProducts(Target target);

        protected Order(string desc, string table, Target type) {
            this.desc = desc;
            this.table = table;
            this.type = type;
            this.state = State.NotPicked;
        }
    
        public static Order NewOrder(string desc, string table, Target type, Dictionary<Product, uint> items) {
            if (type == Target.Both) {
                return new CompositeOrder(desc, table, type, items);
            }
            return new SimpleOrder(desc, table, type, items);
        }

        public Tuple<long, string> ToTuple() {
            return new Tuple<long, string>(this.id, this.desc);
        }
    }


    public class CompositeOrder: Order {
        Order bar_order;
        Order kit_order;

        internal CompositeOrder(string desc, string table, Target type, Dictionary<Product, uint> items)
            :base (desc, table, type) 
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

            this.bar_order = new SimpleOrder(this.id, this.desc, this.table, Target.Bar, drinks);
            this.kit_order = new SimpleOrder(this.id, this.desc, this.table, Target.Kitchen, dishes);
        }

        public override Dictionary<Product, uint> GetProducts(Target target) {
            Dictionary<Product, uint> result = null;
            if (target == Target.Kitchen) {
                result = this.kit_order.GetProducts(target);
            }
            else if (target == Target.Bar) {
                result = this.bar_order.GetProducts(target);
            }
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
    }

    public class SimpleOrder: Order {
        Dictionary<Product, uint> items {get;}

        internal SimpleOrder(long id, string desc, string table, Target type, Dictionary<Product, uint> items)
            :base (desc, table, type) 
        {
            this.id = id;
        }

        internal SimpleOrder(string desc, string table, Target type, Dictionary<Product, uint> items)
            :base (desc, table, type) 
        {
            this.id = Order.curr_id;
            Order.curr_id++;
        }

        public override Dictionary<Product, uint> GetProducts(Target target) { 
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
    
        public override State GetState() {
            return this.state;
        }
    }
}