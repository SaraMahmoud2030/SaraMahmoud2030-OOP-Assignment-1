using System;
using System.Collections.Generic;

public class Customer
{
    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string City { get; }
    public bool IsVip { get; }

    public Customer(int id, string name, string email, string city, bool isVip)
    {
        Id = id;
        Name = name;
        Email = email;
        City = city;
        IsVip = isVip;
    }
}

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Stock { get; private set; }

    public Product(int id, string name, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public bool ReduceStock(int quantity)
    {
        if (quantity <= 0 || quantity > Stock)
            return false;

        Stock -= quantity;
        return true;
    }
}

public class OrderLine
{
    public Product Product { get; }
    public int Quantity { get; }

    public OrderLine(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public decimal GetLineTotal()
    {
        return Product.Price * Quantity;
    }
}

public class Order
{
    public int Id { get; }
    public Customer Customer { get; }
    public DateTime Date { get; }
    public bool IsPaid { get; private set; }

    private readonly List<OrderLine> lines = new();

    public IReadOnlyList<OrderLine> Lines => lines;

    public Order(int id, Customer customer, DateTime date)
    {
        Id = id;
        Customer = customer;
        Date = date;
        IsPaid = false;
    }

    public bool AddLine(Product product, int quantity)
    {
        if (IsPaid || quantity <= 0)
            return false;

        if (!product.ReduceStock(quantity))
            return false;

        lines.Add(new OrderLine(product, quantity));
        return true;
    }

    public decimal CalculateTotal()
    {
        decimal total = 0;

        foreach (OrderLine line in lines)
        {
            total += line.GetLineTotal();
        }

        if (Customer.IsVip)
            total *= 0.90m;

        return total;
    }

    public bool MarkAsPaid()
    {
        if (lines.Count == 0)
            return false;

        IsPaid = true;
        return true;
    }
}

public class OrderSystem
{
    private readonly List<Customer> customers = new();
    private readonly List<Product> products = new();
    private readonly List<Order> orders = new();

    public void AddCustomer(Customer customer)
    {
        if (customers.Exists(c => c.Id == customer.Id))
        {
            Console.WriteLine("Customer ID already exists.");
            return;
        }

        customers.Add(customer);
    }

    public void AddProduct(Product product)
    {
        if (products.Exists(p => p.Id == product.Id))
        {
            Console.WriteLine("Product ID already exists.");
            return;
        }

        products.Add(product);
    }

    public Customer? FindCustomer(int id)
    {
        return customers.Find(c => c.Id == id);
    }

    public Product? FindProduct(int id)
    {
        return products.Find(p => p.Id == id);
    }

    public Order? FindOrder(int id)
    {
        return orders.Find(o => o.Id == id);
    }

    public bool CreateOrder(int id, int customerId, DateTime date)
    {
        if (FindOrder(id) != null)
            return false;

        Customer? customer = FindCustomer(customerId);

        if (customer == null)
            return false;

        orders.Add(new Order(id, customer, date));
        return true;
    }

    public decimal GetTotalPaidSales()
    {
        decimal total = 0;

        foreach (Order order in orders)
        {
            if (order.IsPaid)
                total += order.CalculateTotal();
        }

        return total;
    }
    public void PrintCustomers()
    {
        foreach (Customer customer in customers)
        {
            Console.WriteLine(
                $"{customer.Id} - {customer.Name} - {customer.Email} - {customer.City} - VIP: {customer.IsVip}");
        }
    }

    public void PrintProducts()
    {
        foreach (Product product in products)
        {
            Console.WriteLine(
                $"{product.Id} - {product.Name} - Price: {product.Price} - Stock: {product.Stock}");
        }
    }

    public void PrintOrder(Order order)
    {
        Console.WriteLine($"Order ID: {order.Id}");
        Console.WriteLine($"Customer: {order.Customer.Name}");
        Console.WriteLine($"Date: {order.Date:yyyy-MM-dd}");
        Console.WriteLine($"Paid: {order.IsPaid}");

        foreach (OrderLine line in order.Lines)
        {
            Console.WriteLine(
                $"{line.Product.Name} x {line.Quantity} = {line.GetLineTotal()}");
        }

        Console.WriteLine($"Total: {order.CalculateTotal()}");
    }
}
//=================================
public class Program
{
    public static void Main()
    {
        OrderSystem system = new OrderSystem();

        SeedData(system);

        while (true)
        {
            PrintMenu();

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    system.PrintCustomers();
                    break;

                case "2":
                    system.PrintProducts();
                    break;

                case "3":
                    PrintAllOrders(system);
                    break;

                case "4":
                    PrintOneOrder(system);
                    break;

                case "5":
                    CreateOrder(system);
                    break;

                case "6":
                    AddLine(system);
                    break;

                case "7":
                    MarkOrderPaid(system);
                    break;

                case "8":
                    Console.WriteLine(
                        $"Total Paid Sales: {system.GetTotalPaidSales()}");
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }

    static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("===== ORDER SYSTEM =====");
        Console.WriteLine("1. Show Customers");
        Console.WriteLine("2. Show Products");
        Console.WriteLine("3. Show All Orders");
        Console.WriteLine("4. Show One Order");
        Console.WriteLine("5. Create Order");
        Console.WriteLine("6. Add Product To Order");
        Console.WriteLine("7. Mark Order As Paid");
        Console.WriteLine("8. Show Total Paid Sales");
        Console.WriteLine("0. Exit");
        Console.WriteLine("========================");
    }

    static void SeedData(OrderSystem system)
    {
        system.AddCustomer(
            new Customer(1, "Mona Ali", "mona@example.com", "Cairo", true));

        system.AddCustomer(
            new Customer(2, "Omar Hassan", "omar@example.com", "Alexandria", false));

        system.AddCustomer(
            new Customer(3, "Sara Nabil", "sara@example.com", "Giza", false));

        system.AddProduct(
            new Product(101, "USB Cable", 50, 100));

        system.AddProduct(
            new Product(102, "Wireless Mouse", 250, 40));

        system.AddProduct(
            new Product(103, "Mechanical Keyboard", 1200, 15));

        system.AddProduct(
            new Product(104, "Laptop Stand", 400, 25));

        system.CreateOrder(1001, 1, new DateTime(2026, 9, 15));

        Order? order1 = system.FindOrder(1001);

        if (order1 != null)
        {
            Product? usb = system.FindProduct(101);
            Product? mouse = system.FindProduct(102);

            if (usb != null)
                order1.AddLine(usb, 2);

            if (mouse != null)
                order1.AddLine(mouse, 1);

            order1.MarkAsPaid();
        }

        system.CreateOrder(1002, 2, new DateTime(2026, 9, 15));

        Order? order2 = system.FindOrder(1002);

        if (order2 != null)
        {
            Product? keyboard = system.FindProduct(103);
            Product? stand = system.FindProduct(104);

            if (keyboard != null)
                order2.AddLine(keyboard, 1);

            if (stand != null)
                order2.AddLine(stand, 1);
        }

        system.CreateOrder(1003, 3, new DateTime(2026, 9, 15));

        Order? order3 = system.FindOrder(1003);

        if (order3 != null)
        {
            Product? usb = system.FindProduct(101);

            if (usb != null)
                order3.AddLine(usb, 5);

            order3.MarkAsPaid();
        }
    }
    static void PrintAllOrders(OrderSystem system)
    {
        for (int id = 1001; id <= 1003; id++)
        {
            Order? order = system.FindOrder(id);

            if (order != null)
            {
                system.PrintOrder(order);
                Console.WriteLine("--------------------");
            }
        }
    }

    static void PrintOneOrder(OrderSystem system)
    {
        Console.Write("Enter Order ID: ");
        int id = int.Parse(Console.ReadLine()!);

        Order? order = system.FindOrder(id);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        system.PrintOrder(order);
    }

    static void CreateOrder(OrderSystem system)
    {
        Console.Write("Order ID: ");
        int orderId = int.Parse(Console.ReadLine()!);

        Console.Write("Customer ID: ");
        int customerId = int.Parse(Console.ReadLine()!);

        bool created = system.CreateOrder(
            orderId,
            customerId,
            DateTime.Now);

        Console.WriteLine(
            created ? "Order created successfully." : "Failed to create order.");
    }

    static void AddLine(OrderSystem system)
    {
        Console.Write("Order ID: ");
        int orderId = int.Parse(Console.ReadLine()!);

        Console.Write("Product ID: ");
        int productId = int.Parse(Console.ReadLine()!);

        Console.Write("Quantity: ");
        int quantity = int.Parse(Console.ReadLine()!);

        Order? order = system.FindOrder(orderId);
        Product? product = system.FindProduct(productId);

        if (order == null || product == null)
        {
            Console.WriteLine("Order or product not found.");
            return;
        }

        bool added = order.AddLine(product, quantity);

        Console.WriteLine(
            added ? "Product added successfully." : "Failed to add product.");
    }

    static void MarkOrderPaid(OrderSystem system)
    {
        Console.Write("Order ID: ");
        int orderId = int.Parse(Console.ReadLine()!);

        Order? order = system.FindOrder(orderId);

        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        bool paid = order.MarkAsPaid();

        Console.WriteLine(
            paid ? "Order marked as paid." : "Cannot pay this order.");
    }
}