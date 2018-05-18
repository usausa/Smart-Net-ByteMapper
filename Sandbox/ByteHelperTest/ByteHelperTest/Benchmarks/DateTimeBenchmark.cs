namespace ByteHelperTest.Benchmarks
{
    using System;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(BenchmarkConfig))]
    public class DateTimeBenchmark
    {
        private static readonly byte[] Bytes17 = Encoding.ASCII.GetBytes("20001231235959");

        private static readonly DateTime Date = new DateTime(2199, 12, 31, 23, 59, 59, 123);

        private ByteHelper4.DateTimeEntry[] entries8B;

        private ByteHelper4.DateTimeEntry[] entries17B;

        //private ByteHelper4.IDateTimeBlockConverter[] converters8A;

        //private ByteHelper4.IDateTimeBlockConverter[] converters17A;

        [GlobalSetup]
        public void Setup()
        {
            //converters8A = new ByteHelper4.IDateTimeBlockConverter[]
            //{
            //    new ByteHelper4.Year4BlockConverter(0),
            //    new ByteHelper4.Month2BlockConverter(4),
            //    new ByteHelper4.Day2BlockConverter(6)
            //};
            //converters17A = new ByteHelper4.IDateTimeBlockConverter[]
            //{
            //    new ByteHelper4.Year4BlockConverter(0),
            //    new ByteHelper4.Month2BlockConverter(4),
            //    new ByteHelper4.Day2BlockConverter(6),
            //    new ByteHelper4.Hour2BlockConverter(8),
            //    new ByteHelper4.Minute2BlockConverter(10),
            //    new ByteHelper4.Second2BlockConverter(12),
            //    new ByteHelper4.Millisecond3BlockConverter(14),
            //};
            entries8B = new[]
            {
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Year, 4, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Month, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Day, 2, null)
            };
            entries17B = new[]
            {
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Year, 4, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Month, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Day, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Hour, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Minute, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Second, 2, null),
                new ByteHelper4.DateTimeEntry(ByteHelper4.DateTimePart.Millisecond, 2, null)
            };
        }

        //--------------------------------------------------------------------------------
        // Parse
        //--------------------------------------------------------------------------------

        //[Benchmark]
        //public void ParseDefault17()
        //{
        //    DateTime.TryParseExact(Encoding.ASCII.GetString(Bytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
        //}

        [Benchmark]
        public void ParseCustom8()
        {
            ByteHelper4.TryParseDateTime(Bytes17, 0, "yyyyMMdd", DateTimeKind.Unspecified, out _);
        }

        [Benchmark]
        public void ParseCustom8B()
        {
            ByteHelper4.TryParseDateTime2(Bytes17, 0, entries8B, DateTimeKind.Unspecified, out _);
        }

        [Benchmark]
        public void ParseCustom17()
        {
            ByteHelper4.TryParseDateTime(Bytes17, 0, "yyyyMMddHHmmss", DateTimeKind.Unspecified, out _);
        }

        [Benchmark]
        public void ParseCustom17B()
        {
            ByteHelper4.TryParseDateTime2(Bytes17, 0, entries17B, DateTimeKind.Unspecified, out _);
        }

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

        //[Benchmark]
        //public void FormatCustom8B()
        //{
        //    ByteHelper4.FormatDateTime2(Bytes17, 0, true, entries8B, Date);
        //}

        //[Benchmark]
        //public void FormatCustom17()
        //{
        //    ByteHelper4.FormatDateTime(Bytes17, 0, "yyyyMMddHHmmssfff", Date);
        //}

        //[Benchmark]
        //public void FormatCustom17B()
        //{
        //    ByteHelper4.FormatDateTime2(Bytes17, 0, true, entries17B, Date);
        //}

        //--------------------------------------------------------------------------------

        // MEMO slow
        //[Benchmark]
        //public unsafe void FormatCustom8A()
        //{
        //    ByteHelper4.GetDatePart(Date.Ticks, out var year, out var month, out var day);

        //    var context = new ByteHelper4.DateTimeContext
        //    {
        //        Year = year,
        //        Month = month,
        //        Day = day,
        //        Ticks = Date.Ticks
        //    };

        //    fixed (byte* pBytes = &Bytes17[0])
        //    {
        //        for (var i = 0; i < converters8A.Length; i++)
        //        {
        //            converters8A[i].Write(pBytes, ref context);
        //        }
        //    }

        //    ByteHelper4.FormatDateTime(Bytes17, 0, "yyyyMMdd", Date);
        //}

        // MEMO slow
        //[Benchmark]
        //public unsafe void FormatCustom17A()
        //{
        //    ByteHelper4.GetDatePart(Date.Ticks, out var year, out var month, out var day);

        //    var context = new ByteHelper4.DateTimeContext
        //    {
        //        Year = year,
        //        Month = month,
        //        Day = day,
        //        Ticks = Date.Ticks
        //    };

        //    fixed (byte* pBytes = &Bytes17[0])
        //    {
        //        for (var i = 0; i < converters17A.Length; i++)
        //        {
        //            converters17A[i].Write(pBytes, ref context);
        //        }
        //    }

        //    ByteHelper4.FormatDateTime(Bytes17, 0, "yyyyMMdd", Date);
        //}
    }
}
