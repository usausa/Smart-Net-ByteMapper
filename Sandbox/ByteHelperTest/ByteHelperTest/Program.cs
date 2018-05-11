using System.Diagnostics;
using System.Text;

namespace ByteHelperTest
{
    using System.Reflection;

    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Debug.WriteLine(decimal.MaxValue);
            var buffer = new byte[29];
            ByteHelper2.FormatDecimal(buffer, 0, buffer.Length, decimal.MaxValue, 0, -1, Padding.Left, false, 0x20);
            Debug.WriteLine(Encoding.ASCII.GetString(buffer));

            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
