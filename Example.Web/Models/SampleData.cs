namespace Example.Web.Models;

public sealed class SampleData
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public DateTime? Date { get; set; }
}
