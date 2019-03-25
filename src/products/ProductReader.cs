using Base;
using System;
using System.Collections.Generic;

namespace Restaurant {
    /// <summary>
    /// Class to read products from the .csv files
    /// </summary>
    public static class ProductReader {
        private const string STR_START = Constants.ASSETS_DIR + "products/";
        private const string DISHES_FILE = STR_START + "dishes.csv";
        private const string DRINKS_FILE = STR_START + "drinks.csv";

        /// <summary>
        /// Verifies if the read information from the files is in the correct format
        /// </summary>
        /// <param name="f_name">Name of the file that was read</param>
        /// <param name="words">Array of words read from the file</param>
        /// <returns></returns>
        private static bool correctFormat(string f_name, string[] words) {
            double number;
            if (words.Length > 0 && words.Length % 3 == 0) {
                for (int i = 0; i < words.Length; i+=3) {
                    if (!Double.TryParse(words[i+1], out number)) {
                        Console.WriteLine("File '{0}' @ {1} wrong price format!", f_name, i%3 + 1);
                        return false;
                    }

                    if (!Double.TryParse(words[i+2], out number)) {
                        Console.WriteLine("File '{0}' @ {1} wrong time format!", f_name, i%3 + 1);
                        return false;
                    }
                }
                return true;
            }

            Console.WriteLine("File '{0}' has wrong number of words!", f_name);
            return false;
        }

        /// <summary>
        /// Reads the file information from the file system
        /// </summary>
        /// <param name="file_name">Name of file to be read</param>
        /// <returns></returns>
        private static string[][] readFile(string file_name) {
            try {
                string[] lines = System.IO.File.ReadAllLines(file_name);
                string[][] products = new string[lines.Length][];
                
                for (int i = 0; i < lines.Length; i++) {
                    string[] words = lines[i].Split(';');
                    if (!correctFormat(file_name, words)) {
                        return null;
                    }
                    products[i] = words;
                }
                return products;
            }
            catch (Exception e) {
                Console.WriteLine("Error reading file '{0}'\n - {1}", file_name, e);
            }
                return null;
        }

        /// <summary>
        /// Converts a list of words to a Product
        /// </summary>
        /// <param name="words">List of words to use to construct a product</param>
        /// <param name="is_dish">Whether the product is a dish or not</param>
        /// <returns>List of products</returns>
        private static List<Product> toProducts(string[][] words, bool is_dish) {
            List<Product> products = new List<Product>(words.Length);
            for (int i = 0; i < words.Length; i++) {
                products.Add(new Product(words[i][0], 
                        double.Parse(words[i][1], System.Globalization.CultureInfo.InvariantCulture), 
                        double.Parse(words[i][2], System.Globalization.CultureInfo.InvariantCulture),
                        is_dish));
            }
            return products;
        }

        /// <summary>
        /// Reads the dishes from the file
        /// </summary>
        /// <returns>List of products read from the file</returns>
        public static List<Product> ReadDishes() {
            string[][] words = readFile(DISHES_FILE);
            if (words != null) {
                return toProducts(words, true);
            }
            return null;
        }

        /// <summary>
        /// Reads the drinks from the file
        /// </summary>
        /// <returns>List of products read from the file</returns>
        public static List<Product> ReadDrinks() {
            string[][] words = readFile(DRINKS_FILE);
            if (words != null) {
                return toProducts(words, false);
            }
            return null;
        }        
    }
}