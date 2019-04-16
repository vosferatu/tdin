using Base;
using System;

namespace Restaurant {
    [Serializable]
    /// <summary>
    /// Represents a single product, which is either a drink or a dish
    /// </summary>
    public class Product: IEquatable<Product>{
        public string name {get; private set;}
        public double price {get; private set;}
        public double time {get; private set;}
        public ProductType type{get; private set;}

        /// <summary>
        /// Default constructor for the product
        /// </summary>
        /// <param name="name">Name of the product</param>
        /// <param name="price">Price of the product</param>
        /// <param name="time">Time of confection of the product</param>
        /// <param name="is_dish">Whether the product is a dish or not</param>
        public Product(string name, double price, double time, bool is_dish) {
            this.name = name;
            this.price = price;
            this.time = time;
            this.type = (is_dish) ? ProductType.Dish : ProductType.Drink;
        }

        /// <summary>
        /// Implementation of the object.GetHashCode(), used in Dictionaries
        /// </summary>
        /// <returns>Products name hash coded</returns>
        public override int GetHashCode() {
            return this.name.GetHashCode();
        }
        
        /// <summary>
        /// Checks whether this product is equal to some other
        /// </summary>
        /// <param name="other">Other product to be checked</param>
        /// <returns>Whether the products are equal or not</returns>
        public bool Equals(Product other) {
            return this.name == other.name;
        }

        /// <summary>
        /// Generic implementation of the Equals method
        /// </summary>
        /// <param name="obj">Other object to compare</param>
        /// <returns>Whether the product is equal to the given object or not</returns>
        public override bool Equals(object obj) {
            return ((obj is Product) && Equals((Product)obj, this));
        }
    }
}