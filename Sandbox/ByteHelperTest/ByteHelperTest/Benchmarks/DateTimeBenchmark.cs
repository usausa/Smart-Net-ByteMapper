namespace ByteHelperTest.Benchmarks;

using System;
using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

[Config(typeof(BenchmarkConfig))]
public class DateTimeBenchmark
{
    private static readonly byte[] Bytes17 = Encoding.ASCII.GetBytes("20001231235959999");

    private static readonly DateTime Date = new DateTime(2199, 12, 31, 23, 59, 59, 123);

    private ByteHelper4.DateTimeEntry[] entries8B;

    private ByteHelper4.DateTimeEntry[] entries17B;

    [GlobalSetup]
    public void Setup()
    {
        entries8B = ByteHelper4.ParseDateTimeFormat("yyyyMMdd", out _);
        entries17B = ByteHelper4.ParseDateTimeFormat("yyyyMMddHHmmssfff", out _);
    }

    //--------------------------------------------------------------------------------
    // Parse
    //--------------------------------------------------------------------------------

    //[Benchmark]
    //public void ParseDefault17()
    //{
    //    DateTime.TryParseExact(Encoding.ASCII.GetString(Bytes17), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
    //}

    //[Benchmark]
    //public void ParseCustom8()
    //{
    //    ByteHelper4.TryParseDateTime(Bytes17, 0, "yyyyMMdd", DateTimeKind.Unspecified, out _);
    //}

    //[Benchmark]
    //public void ParseCustom8B()
    //{
    //    ByteHelper4.TryParseDateTime2(Bytes17, 0, entries8B, DateTimeKind.Unspecified, out _);
    //}

    //[Benchmark]
    //public void ParseCustom17()
    //{
    //    ByteHelper4.TryParseDateTime(Bytes17, 0, "yyyyMMddHHmmssfff", DateTimeKind.Unspecified, out _);
    //}

    //[Benchmark]
    //public void ParseCustom17B()
    //{
    //    ByteHelper4.TryParseDateTime2(Bytes17, 0, entries17B, DateTimeKind.Unspecified, out _);
    //}

    //--------------------------------------------------------------------------------
    // Format
    //--------------------------------------------------------------------------------

    //[Benchmark]
    //public void FormatDefault17()
    //{
    //    Encoding.ASCII.GetBytes(Date.ToString("yyyyMMddHHmmssfff"));
    //}

    //[Benchmark]
    //public void FormatCustom8()
    //{
    //    ByteHelper4.FormatDateTime(Bytes17, 0, "yyyyMMdd", Date);
    //}

    [Benchmark]
    public void FormatCustom8B()
    {
        ByteHelper4.FormatDateTime2(Bytes17, 0, true, entries8B, Date);
    }

    //[Benchmark]
    //public void FormatCustom17()
    //{
    //    ByteHelper4.FormatDateTime(Bytes17, 0, "yyyyMMddHHmmssfff", Date);
    //}

    [Benchmark]
    public void FormatCustom17B()
    {
        ByteHelper4.FormatDateTime2(Bytes17, 0, true, entries17B, Date);
    }
}
