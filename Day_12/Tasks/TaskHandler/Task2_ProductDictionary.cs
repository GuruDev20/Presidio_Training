using System;
using System.Collections.Generic;

namespace Tasks.TaskHandler
{
    public class Task2_ProductDictionary
    {
        public static void Run()
        {
            Dictionary<string,double> productPrices=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);
            AddProducts(productPrices);
            DisplayProducts(productPrices);
            SearchProduct(productPrices);
        }

        public static void AddProducts(Dictionary<string,double> productPrices)
        {
            Console.WriteLine("Add 5 products (name + price):");
            for (int i = 0; i < 5; i++)
            {
                string name;
                while (true)
                {
                    Console.Write($"Enter name for product {i + 1}: ");
                    name = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Product name cannot be empty.");
                    }
                    else if (productPrices.ContainsKey(name))
                    {
                        Console.WriteLine("Product already exists. Enter a different name.");
                    }
                    else
                    {
                        break;
                    }
                }
                double price;
                Console.Write($"Enter price for {name}: ");
                while (!double.TryParse(Console.ReadLine(), out price) || price < 0)
                {
                    Console.Write("Invalid price. Enter a non-negative number: ");
                }

                productPrices.Add(name, price);
            }
        }

        public static void DisplayProducts(Dictionary<string,double> productPrices)
        {
            Console.WriteLine("\n--- Product List ---");
            foreach(var item in productPrices)
            {
                Console.WriteLine($"Product: {item.Key} | Price: {item.Value}");
            }
        }

        public static void SearchProduct(Dictionary<string,double> productPrices)
        {
            Console.Write("\nEnter a product name to search: ");
            string search = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }
            if (productPrices.ContainsKey(search))
            {
                Console.WriteLine($"The price of {search} is ₹{productPrices[search]}");
            }
            else
            {
                Console.WriteLine($"{search} not found in product list.");
            }
        }
    }
}
