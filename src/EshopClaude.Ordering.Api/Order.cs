namespace EshopClaude.Ordering.Api;

public class Order
{
    public int Id { get; set; }

    public string BuyerId { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public decimal Total { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}
