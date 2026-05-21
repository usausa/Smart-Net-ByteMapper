namespace Example.Web.Models;

using Smart.IO.ByteMapper;

// Layout: 59 bytes (57 data + 2 bytes CRLF delimiter)
// Code(0,13) + Name(13,20) + Qty(33,6) + Price(39,10) + Date(49,8)
[Map(59)]
public sealed class SampleData
{
    [MapText(0, 13)]
    public string Code { get; set; } = default!;

    [MapText(13, 20, CodePage = 932)]
    public string Name { get; set; } = default!;

    [MapNumberText<int>(33, 6)]
    public int Qty { get; set; }

    [MapNumberText<decimal>(39, 10, Style = System.Globalization.NumberStyles.Number)]
    public decimal Price { get; set; }

    [MapDateTimeText<DateTime>(49, 8, "yyyyMMdd")]
    public DateTime Date { get; set; }
}
