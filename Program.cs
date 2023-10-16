public class Product
{
    public string Name { get; set; }
    public string Barcode { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime ExpiryDate { get; set; }

    public decimal TotalValue => Quantity * Price;
}

public class InventoryManager
{
    private List<Product> _products = new List<Product>();

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public void UpdateProduct(string barcode, Product updatedProduct)
    {
        var product = _products.FirstOrDefault(p => p.Barcode == barcode);
        if (product != null)
        {
            product.Name = updatedProduct.Name;
            product.Quantity = updatedProduct.Quantity;
            product.Price = updatedProduct.Price;
            product.ExpiryDate = updatedProduct.ExpiryDate;
        }
    }

    public void RemoveProduct(string barcode)
    {
        _products.RemoveAll(p => p.Barcode == barcode);
    }

    public Product FindByBarcode(string barcode)
    {
        return _products.FirstOrDefault(p => p.Barcode == barcode);
    }

    public IEnumerable<Product> FindByName(string name)
    {
        return _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Product> FindByExpiryDate(DateTime date)
    {
        return _products.Where(p => p.ExpiryDate <= date);
    }

    public decimal CalculateTotalValue()
    {
        return _products.Sum(p => p.TotalValue);
    }

    public IEnumerable<Product> GetExpiringProducts(int days)
    {
        var targetDate = DateTime.Now.AddDays(days);
        return _products.Where(p => p.ExpiryDate <= targetDate).OrderBy(p => p.ExpiryDate);
    }
}

class Program
{
    static InventoryManager manager = new InventoryManager();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1. Adicionar produto");
            Console.WriteLine("2. Atualizar produto");
            Console.WriteLine("3. Remover produto");
            Console.WriteLine("4. Buscar produto");
            Console.WriteLine("5. Valor total do estoque");
            Console.WriteLine("6. Relatório de produtos prestes a vencer");
            Console.WriteLine("7. Sair");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    try
                    {
                        AddProduct();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "2":
                    try
                    {
                        UpdateProduct();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "3":
                    try
                    {
                        RemoveProduct();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "4":
                    try
                    {
                        SearchProduct();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "5":
                    try
                    {
                        CalculateTotalValue();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "6":
                    try
                    {
                        DisplayExpiringProducts();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }
    static void AddProduct()
    {
        Console.Write("Nome do produto: ");
        string name = Console.ReadLine();
        Console.Write("Código de barras: ");
        string barcode = Console.ReadLine();
        Console.Write("Quantidade: ");
        int quantity = int.Parse(Console.ReadLine());
        Console.Write("Preço unitário: ");
        decimal price = decimal.Parse(Console.ReadLine());
        Console.Write("Data de validade (dd/MM/yyyy): ");
        DateTime expiryDate = DateTime.Parse(Console.ReadLine());

        var product = new Product
        {
            Name = name,
            Barcode = barcode,
            Quantity = quantity,
            Price = price,
            ExpiryDate = expiryDate
        };

        manager.AddProduct(product);
        Console.WriteLine("Produto adicionado com sucesso!");
    }

    static void UpdateProduct()
    {
        Console.Write("Código de barras do produto a ser atualizado: ");
        string barcode = Console.ReadLine();
        var existingProduct = manager.FindByBarcode(barcode);
        if (existingProduct == null)
        {
            Console.WriteLine("Produto não encontrado!");
            return;
        }

        Console.Write($"Nome do produto ({existingProduct.Name}): ");
        string name = Console.ReadLine();
        Console.Write($"Quantidade ({existingProduct.Quantity}): ");
        int quantity = int.Parse(Console.ReadLine());
        Console.Write($"Preço unitário ({existingProduct.Price}): ");
        decimal price = decimal.Parse(Console.ReadLine());
        Console.Write($"Data de validade ({existingProduct.ExpiryDate:dd/MM/yyyy}): ");
        DateTime expiryDate = DateTime.Parse(Console.ReadLine());

        var updatedProduct = new Product
        {
            Name = string.IsNullOrWhiteSpace(name) ? existingProduct.Name : name,
            Quantity = quantity,
            Price = price,
            ExpiryDate = expiryDate
        };

        manager.UpdateProduct(barcode, updatedProduct);
        Console.WriteLine("Produto atualizado com sucesso!");
    }

    static void RemoveProduct()
    {
        Console.Write("Código de barras do produto a ser removido: ");
        string barcode = Console.ReadLine();
        manager.RemoveProduct(barcode);
        Console.WriteLine("Produto removido com sucesso!");
    }

    static void SearchProduct()
    {
        Console.WriteLine("Buscar por: 1. Nome, 2. Código de barras, 3. Data de validade");
        string choice = Console.ReadLine();
        IEnumerable<Product> results = null;

        switch (choice)
        {
            case "1":
                Console.Write("Nome: ");
                string name = Console.ReadLine();
                results = manager.FindByName(name);
                break;
            case "2":
                Console.Write("Código de barras: ");
                string barcode = Console.ReadLine();
                var product = manager.FindByBarcode(barcode);
                if (product != null) results = new List<Product> { product };
                break;
            case "3":
                Console.Write("Data de validade (dd/MM/yyyy): ");
                DateTime expiryDate = DateTime.Parse(Console.ReadLine());
                results = manager.FindByExpiryDate(expiryDate);
                break;
            default:
                Console.WriteLine("Opção inválida!");
                return;
        }

        Console.WriteLine("Produtos encontrados:");
        foreach (var product in results)
        {
            DisplayProduct(product);
        }
    }

    static void CalculateTotalValue()
    {
        decimal total = manager.CalculateTotalValue();
        Console.WriteLine($"Valor total do estoque: {total:C2}");
    }

    static void DisplayExpiringProducts()
    {
        Console.Write("Dias até a data de validade: ");
        int days = int.Parse(Console.ReadLine());
        var products = manager.GetExpiringProducts(days);

        Console.WriteLine($"Produtos prestes a vencer em {days} dias:");
        foreach (var product in products)
        {
            DisplayProduct(product);
        }
    }

    static void DisplayProduct(Product product)
    {
        Console.WriteLine($"\nNome: {product.Name}");
        Console.WriteLine($"Código de Barras: {product.Barcode}");
        Console.WriteLine($"Quantidade: {product.Quantity}");
        Console.WriteLine($"Preço Unitário: {product.Price:C2}");
        Console.WriteLine($"Data de Validade: {product.ExpiryDate:dd/MM/yyyy}");
    }
}