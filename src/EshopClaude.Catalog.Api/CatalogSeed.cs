namespace EshopClaude.Catalog.Api;

internal static class CatalogSeed
{
    public static void Seed(CatalogDbContext db)
    {
        if (db.Products.Any())
        {
            return;
        }

        db.Products.AddRange(
            Create("Classic Mug", "A 350 ml ceramic mug for coffee or tea.", 12.50m),
            Create("Canvas Tote Bag", "A sturdy cotton canvas bag for everyday carry.", 18.00m),
            Create("Cotton T-Shirt", "A soft, regular-fit t-shirt in 100% cotton.", 24.99m),
            Create("Hoodie", "A warm fleece-lined hoodie with a front pocket.", 49.99m),
            Create("Baseball Cap", "An adjustable six-panel cap with a curved brim.", 19.95m),
            Create("Notebook", "An A5 dotted notebook with 120 pages.", 9.99m),
            Create("Water Bottle", "A 750 ml insulated steel bottle.", 22.00m),
            Create("Sticker Pack", "A pack of 10 vinyl stickers.", 4.50m));

        db.SaveChanges();
    }

    private static Product Create(string name, string description, decimal price) => new()
    {
        Name = name,
        Description = description,
        Price = price,
        ImageUrl = $"https://placehold.co/600x400?text={Uri.EscapeDataString(name).Replace("%20", "+")}"
    };
}
