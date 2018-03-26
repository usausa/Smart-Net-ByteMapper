namespace SjisTest
{
    using System.Diagnostics;
    using System.Text;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding(932);
            var bytes = sjis.GetBytes("123456789");

            Debug.WriteLine(SjisHelper.ParseInt(bytes, 0, bytes.Length));
        }
    }
}
