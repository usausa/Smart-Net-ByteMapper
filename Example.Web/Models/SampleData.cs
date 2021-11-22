namespace Example.Web.Models;

using System;
using System.Diagnostics.CodeAnalysis;

public class SampleData
{
    [AllowNull]
    public string Code { get; set; }

    [AllowNull]
    public string Name { get; set; }

    public int Qty { get; set; }

    public decimal Price { get; set; }

    public DateTime? Date { get; set; }
}
