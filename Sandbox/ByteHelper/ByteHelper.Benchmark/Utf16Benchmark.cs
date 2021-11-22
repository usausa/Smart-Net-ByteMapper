namespace ByteHelper;

using System.Text;

using BenchmarkDotNet.Attributes;

public class Utf16Benchmark
{
    private const string Value = "あいうえお12345     ";

    private byte[] buffer;

    [GlobalSetup]
    public void Setup()
    {
        buffer = Encoding.Unicode.GetBytes(Value);
    }

    //[Benchmark]
    //public void GetStringDefault()
    //{
    //    Encoding.Unicode.GetString(buffer, 0, buffer.Length);
    //}

    [Benchmark]
    public void GetStringCustom()
    {
        ByteHelper3.GetUtf16String(buffer, 0, buffer.Length, Padding.Right, ' ');
    }

    //[Benchmark]
    //public void CopyBytesDefault()
    //{
    //    Encoding.Unicode.GetBytes(Value.Trim());
    //}

    [Benchmark]
    public void CopyBytesCustom()
    {
        ByteHelper3.CopyUtf16Bytes(Value, buffer, 0, buffer.Length, Padding.Right, ' ');
    }
}
