using NSE.Core.Data;

namespace NSE.Catalog.API.Models;

public class Product : IAggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Image { get; set; }
    public int StockQuantity { get; set; }
}
